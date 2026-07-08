# DevOps TP API

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AgusElmer_devops-tp-api&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=AgusElmer_devops-tp-api)

API desarrollada para el trabajo práctico integrador de DevOps.

El objetivo del proyecto es construir una API simple y utilizarla como base para aplicar prácticas DevOps:

- pruebas automatizadas
- Docker
- Docker Compose
- GitHub Actions
- publicación de imagen Docker
- despliegue continuo
- monitoreo
- releases y rollback

## Stack

- .NET 8
- Minimal API
- xUnit
- Swagger / OpenAPI
- Docker
- Docker Compose
- GitHub Actions
- Docker Hub
- Render
- SonarCloud
- OpenTelemetry
- New Relic

## Endpoints generales

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/` | Información general de la API |
| GET | `/health` | Health check técnico |
| GET | `/ready` | Readiness check |
| GET | `/version` | Versión de la API |
| GET | `/diagnostics/ping` | Endpoint simple de diagnóstico |
| GET | `/diagnostics/error` | Error controlado para monitoreo |
| GET | `/diagnostics/slow` | Endpoint lento para pruebas de APM |

## Endpoints RPG Quests

La API incluye un módulo in-memory de quests RPG. No utiliza base de datos: los datos se pierden al reiniciar la aplicación.

| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/quests` | Lista todas las quests |
| GET | `/api/quests/{id}` | Obtiene una quest por id |
| POST | `/api/quests` | Crea una quest con estado `available` |
| PATCH | `/api/quests/{id}/accept` | Cambia una quest `available` a `accepted` |
| PATCH | `/api/quests/{id}/complete` | Cambia una quest `accepted` a `completed` |
| PATCH | `/api/quests/{id}/abandon` | Cambia una quest `accepted` a `abandoned` |
| GET | `/api/quests/summary` | Devuelve resumen por estado, rango y recompensas completadas |

Valores permitidos:

- `rank`: `E`, `D`, `C`, `B`, `A`, `S`
- `type`: `combat`, `gathering`, `exploration`, `delivery`, `rescue`, `crafting`
- `status`: `available`, `accepted`, `completed`, `abandoned`

## Ejecutar localmente

```bash
dotnet restore
dotnet build
dotnet test
dotnet run --project src/DevOpsTp.Api
```

## Ejecutar con Docker

```bash
docker build -t devops-tp-api .
docker run --rm -p 8080:8080 devops-tp-api
```

Con Docker Compose:

```bash
docker compose up --build
```

La API queda disponible en:

```txt
http://localhost:8080
```

## Swagger

Una vez levantada la API, abrir:

```txt
http://localhost:<puerto>/swagger
```

## CI/CD

El repositorio utiliza GitHub Actions:

- CI en pull requests hacia `develop` y `main`.
- Build y tests automatizados.
- Análisis de calidad y seguridad con SonarCloud (Quality Gate como check del PR; ver `docs/sonarcloud.md`).
- Validación de flujo de mergeo hacia `main`.
- CD en push a `main`.
- Generación de versión semántica (según `#patch`, `#minor`, `#major` o `#none` en el mensaje de commit).
- Build de imagen Docker.
- Publicación de imagen en Docker Hub (`<usuario>/devops-tp-api:<version>` y `:latest`).
- Deploy continuo en Render mediante deploy hook con `imgURL`.
- Backport automático desde `main` hacia `develop` (PR automático que dispara CI).
- Rollback manual a cualquier versión publicada (workflow `Rollback - Render`).

### Secrets requeridos en GitHub Actions

Configurar en el repositorio (Settings → Secrets and variables → Actions), sin valores en el código:

| Secret | Uso |
|---|---|
| `DOCKERHUB_USERNAME` | Usuario de Docker Hub, también define el nombre de la imagen |
| `DOCKERHUB_TOKEN` | Access token de Docker Hub con permisos Read/Write |
| `RENDER_DEPLOY_HOOK_URL` | Deploy hook del servicio en Render |
| `BACKPORT_TOKEN` | Fine-grained PAT (Contents RW, Pull requests RW, Issues RW, Metadata RO) para el PR automático de backport |
| `SONAR_TOKEN` | Token de SonarCloud para el análisis de calidad en CI (ver `docs/sonarcloud.md`) |

La license key de New Relic **no** va en GitHub: se configura como variable de entorno en Render (ver `docs/monitoreo.md`).

## Deploy

Ambiente publicado en Render:

```txt
https://devops-tp-api.onrender.com
```

Endpoints útiles para validar el deploy:

```bash
curl https://devops-tp-api.onrender.com/health
curl https://devops-tp-api.onrender.com/version
curl https://devops-tp-api.onrender.com/api/quests
curl https://devops-tp-api.onrender.com/api/quests/summary
```

El servicio en Render se crea como Web Service a partir de una imagen existente de Docker Hub (`docker.io/<usuario>/devops-tp-api:latest`), con puerto 8080 y health check en `/health`. Las variables de entorno de monitoreo se detallan en `docs/monitoreo.md`.

### Rollback

Si una versión deployada presenta problemas (por ejemplo, errores detectados en New Relic), se puede volver a cualquier versión anterior publicada en Docker Hub sin revertir código:

1. Ir a **Actions → Rollback - Render → Run workflow**.
2. Ingresar la versión a restaurar (ej. `0.1.1`).
3. El workflow valida que la imagen exista en Docker Hub y dispara el deploy hook de Render con esa imagen (`imgURL`).
4. Verificar con `curl https://devops-tp-api.onrender.com/version` que la versión activa sea la esperada.

Como cada versión queda inmutable en Docker Hub gracias al versionado semántico del CD, el rollback es determinístico: se redeploya exactamente el mismo artefacto que se construyó en su momento.

## Monitoreo

La API está instrumentada con OpenTelemetry para exportar métricas y trazas a New Relic mediante OTLP.

La configuración de variables de entorno y endpoints de validación está documentada en:

```txt
docs/monitoreo.md
```

## Estado del proyecto

- [x] Esqueleto de API
- [x] Health check
- [x] Swagger
- [x] Tests básicos
- [x] Módulo funcional concreto: RPG Quests
- [x] Dockerfile multi-stage
- [x] Docker Compose
- [x] CI con build y tests
- [x] Flujo de ramas y PRs
- [x] CD con publicación de imagen Docker
- [x] Deploy en Render
- [x] Monitoreo base con OpenTelemetry y New Relic
- [x] Semantic versioning
- [x] `/version` con commit, build date y versión semántica real del deploy
- [x] Backport automático `main` → `develop`
- [x] Rollback de la versión deployada vía workflow manual
- [x] Análisis de calidad y seguridad con SonarCloud (Quality Gate en PRs)

## Checklist de entrega

- [ ] Repo en GitHub con ramas `main` y `develop` protegidas (PR obligatorio + check `Build & Test`).
- [ ] Secrets configurados: `DOCKERHUB_USERNAME`, `DOCKERHUB_TOKEN`, `RENDER_DEPLOY_HOOK_URL`, `BACKPORT_TOKEN`, `SONAR_TOKEN`.
- [ ] CI verde en un PR hacia `develop`.
- [ ] CD verde en push a `main`: tag semántico + imagen en Docker Hub (`:<version>` y `:latest`).
- [ ] Servicio en Render activo con la imagen de Docker Hub y health check en `/health`.
- [ ] Variables de OpenTelemetry configuradas en Render (license key de New Relic solo ahí).
- [ ] Servicio `devops-tp-api` visible en New Relic con throughput, latencia, errores y traces.
- [ ] Capturas de evidencia en `docs/screenshots/` (APM overview, throughput, response time, errors, traces).

## Demo para la presentación

1. Mostrar la API viva: `curl https://devops-tp-api.onrender.com/health` y `/version` (versión, commit y build date reales del deploy).
2. Mostrar el módulo de quests: `GET /api/quests` y `GET /api/quests/summary`.
3. Mostrar el pipeline: crear una rama `release/*` o `hotfix/*`, abrir PR a `main`, mostrar CI (`Build & Test`) y el check de flujo de ramas.
4. Mergear a `main` con `#patch` en el mensaje y mostrar el CD: tag nuevo, push a Docker Hub y deploy hook a Render.
5. Mostrar el PR automático de backport `main` → `develop` con su CI corriendo.
6. Generar tráfico: `./scripts/generate-demo-traffic.sh` (o con `BASE_URL=<url>` para otro ambiente).
7. Mostrar New Relic: APM overview, throughput, response time (pico de `/diagnostics/slow`), errores (`/diagnostics/error`) y un trace.
8. Rollback en vivo ("Andon cord"): ante los errores vistos en New Relic, ejecutar **Actions → Rollback - Render** con la versión anterior y mostrar cómo `/version` vuelve a esa versión en ~1 minuto, sin tocar el código.
<- No se incorpora base de datos en la primera etapa para evitar complejidad demo: corrida de SonarCloud -->
