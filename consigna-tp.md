# Trabajo Práctico — Consigna

## Contexto

Los alumnos tienen que realizar un trabajo práctico integrador en el cual tienen que:

- Desarrollar una API básica.
- Agregar pruebas unitarias.
- Dockerizar la aplicación.
- Orquestación con docker-compose.
- Gestionar el repositorio git y github.
- Configurar un CI y CD.
- Publicar la imagen de docker a docker registry.
- El despliegue continuo a Render u otra plataforma gratuita (no requerido para aprobar).
- Utilizar monitoreo (Datadog, New Relic, Grafana Cloud, Sentry).

La idea principal es que puedan aplicar los conceptos y la teoría de DevOps:

- Three ways
- Andon cords
- Metodologías ágiles
- Lean

Los alumnos hacen grupos de máximo 2 personas o de forma individual. Elaboran un informe del trabajo práctico y realizan una presentación en clase. La evaluación es individual.

Para la fecha final, cada alumno en su rama o fork del repositorio debe agregar una funcionalidad extra relacionada a DevOps.

## Puntaje

| Categoría | Ítem | Puntos |
|-----------|------|--------|
| **Dockerfile** | Dockerfile (requerido) | 1 |
| | Multi-stage image | 1 |
| | Buenas prácticas | 0,5 |
| **Docker Compose** | Buenas prácticas | 0,5 |
| **CI** | Checks (build y unit test) | 0,5 |
| | Flujo de mergeo | 1 |
| **CD** | Publicación dockerfile | 2 |
| | Despliegue a Render u otro servicio | 1 |
| **Monitoreo** | Dashboard (Hits, etc.) | 1,5 |
| | APM / Trazas | 1 |

**Nota máxima: 10**

## Funcionalidades opcionales (no requeridas)

- Crear un status page
- Configurar rama protegida en GitHub (mínimo los checks)
- Workflows reutilizables
- Semantic versioning
- Publicar la imagen a Docker Hub
- Publicar la imagen en GitHub Actions
- Branch de desarrollo
- Hacer releases y notas de release
- Conventional commits
- Kubernetes (limitado por acceso a servicios gratuitos)
- Terraform (Render soporta manejo de infraestructura por Terraform)
- Swagger / OpenAPI
- OWASP API best practices
- Rollback de la versión deployada

## Objetivo

Tener al menos 2 versiones del trabajo práctico: una versión básica y una versión avanzada para quien tiene más experiencia. Los módulos están organizados de forma que a medida que se cumplen los ítems se van sumando puntos hasta la nota máxima de 10.
