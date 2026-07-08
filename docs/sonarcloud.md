# Análisis de calidad con SonarCloud

Este proyecto usa **SonarCloud** para análisis estático de código: detecta bugs,
"code smells", vulnerabilidades de seguridad, código duplicado y mide la
cobertura de tests. Aplica un **Quality Gate** que aparece como check en cada
Pull Request.

El análisis corre por CI (workflow `.github/workflows/sonarcloud.yml`), no por
"Automatic Analysis" de SonarCloud. Esto es obligatorio en proyectos .NET porque
el scanner necesita envolver el build de MSBuild (`begin` → `build` → `test` → `end`).

## Topología

```
Pull Request / push a develop|main
        │
        ▼
GitHub Actions (workflow SonarCloud)
  begin → dotnet build → dotnet test (+cobertura opencover) → end
        │
        ▼
SonarCloud  ──► Quality Gate como check del PR + dashboard + badge
```

## Puesta en marcha (una sola vez)

1. **Crear el proyecto en SonarCloud**
   - Entrar a <https://sonarcloud.io> y loguearse con la cuenta de GitHub.
   - Crear (o elegir) la organización y luego **Analyze new project**, importando
     este repositorio.
   - Anotar los dos valores que genera SonarCloud:
     - **Organization Key** (ej. `aguselmer`)
     - **Project Key** (ej. `AgusElmer_devops-tp-api`)
   - Si difieren de los valores por defecto, actualizarlos en el bloque `env:`
     del workflow (`SONAR_ORG` y `SONAR_PROJECT_KEY`).

2. **Desactivar Automatic Analysis** (imprescindible)
   - En el proyecto: **Administration → Analysis Method → Automatic Analysis: OFF**.
   - Si queda activo, el análisis por CI falla con un error de método duplicado.

3. **Generar el token y cargarlo en GitHub**
   - En SonarCloud: **My Account → Security → Generate Token** (tipo *Project Analysis*
     o *User*).
   - En GitHub: **Settings → Secrets and variables → Actions → New repository secret**
     con nombre **`SONAR_TOKEN`** y el valor del token.

4. **Correr el análisis**
   - Abrir un PR hacia `develop` o `main`, o pushear a esas ramas.
   - El job **Análisis SonarCloud** aparece en Actions y publica el resultado en
     el dashboard de SonarCloud y como check del PR.

## Cobertura de tests

La cobertura se genera con `coverlet.collector` (ya presente en el proyecto de
tests) en formato **opencover**:

```bash
dotnet test --collect:"XPlat Code Coverage;Format=opencover" --results-directory coverage
```

El workflow le pasa a SonarCloud la ruta de los reportes con
`sonar.cs.opencover.reportsPaths`, así el % de cobertura aparece en el dashboard.

## Quality Gate

Por defecto SonarCloud usa el gate *Sonar way*, que evalúa el **código nuevo**
(bugs, vulnerabilidades, cobertura y duplicación sobre lo que cambió en el PR).

El bloqueo de merges **no** lo hace el scanner, sino la **protección de rama** de
GitHub sobre el check nativo de SonarCloud, llamado **`SonarCloud Code Analysis`**
(lo postea la app de SonarCloud en cada PR y refleja el estado del gate). Para
activarlo: **Settings → Branches → regla de `develop`/`main` → Require status
checks → marcar `SonarCloud Code Analysis`** (y `Build & Test`). Así, si el gate
queda en rojo, el PR no se puede mergear.

- Se evita `sonar.qualitygate.wait=true` en el scanner a propósito: ese poll
  falla de forma intermitente en el primer análisis de una rama nueva
  (`Not authorized or project not found`). El check de la app es más confiable.
- El gate y sus umbrales se ajustan desde SonarCloud (**Quality Gates**). Si el
  requisito de 80% de cobertura sobre código nuevo resulta estricto para la
  cursada, se puede clonar el gate y bajar/quitar esa condición.

## Operación diaria

- Cada PR muestra el resumen de SonarCloud (issues nuevos y cobertura del código
  nuevo) como check.
- El dashboard del proyecto queda en
  `https://sonarcloud.io/summary/new_code?id=<SONAR_PROJECT_KEY>`.
- El badge del estado del gate se muestra en el README.

## Notas

- Los PR desde forks no reciben el `SONAR_TOKEN` (por seguridad de GitHub) y por
  lo tanto no se analizan. Para un repo de equipo interno esto no aplica.
- El scanner necesita JDK 17 (lo instala el propio workflow) y build completo
  entre `begin` y `end`.
