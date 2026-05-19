# DevOps TP API

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
- Validación de flujo de mergeo hacia `main`.
- CD en push a `main`.
- Generación de versión semántica.
- Build de imagen Docker.
- Publicación de imagen en Docker Hub.
- Deploy continuo en Render mediante deploy hook.
- Backport automático desde `main` hacia `develop`.

## Deploy

Ambiente publicado en Render:

```txt
https://tp-devops-g3-2026.onrender.com
```

Endpoints útiles para validar el deploy:

```bash
curl https://tp-devops-g3-2026.onrender.com/health
curl https://tp-devops-g3-2026.onrender.com/api/quests
curl https://tp-devops-g3-2026.onrender.com/api/quests/summary
```

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

## Pendientes / mejoras posibles

- Confirmar dashboard de New Relic con evidencia en capturas.
- Documentar rollback operativo de una versión deployada.
- Mejorar `/version` para mostrar commit, build date y versión semántica real del deploy.
- Agregar release notes automáticas.
- Configurar protección de ramas en GitHub si todavía no está aplicada desde la UI.
