# Monitoreo y observabilidad

## Objetivo

El objetivo de esta implementación es incorporar observabilidad a la API utilizando OpenTelemetry y New Relic.

La observabilidad permite analizar el comportamiento de la aplicación a partir de métricas y trazas, facilitando la detección de errores, problemas de latencia y degradaciones del servicio.

## Herramientas utilizadas

- OpenTelemetry
- New Relic
- Render Logs

## Estrategia implementada

La API fue instrumentada con OpenTelemetry para recolectar telemetría de forma estándar y agnóstica al proveedor.

Los datos recolectados pueden exportarse mediante OTLP hacia New Relic, utilizando variables de entorno para configurar el endpoint y la clave de ingesta.

La exportación queda desactivada si no se configura `OTEL_EXPORTER_OTLP_ENDPOINT`. Esto permite ejecutar la API localmente y en tests sin depender de New Relic.

## Variables de entorno

En Render se deben configurar estas variables:

```txt
OTEL_SERVICE_NAME=devops-tp-api
OTEL_EXPORTER_OTLP_ENDPOINT=https://otlp.nr-data.net:443
OTEL_EXPORTER_OTLP_HEADERS=api-key=<NEW_RELIC_LICENSE_KEY>
OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf
```

Para cuentas New Relic de la region EU, el endpoint puede cambiar a:

```txt
OTEL_EXPORTER_OTLP_ENDPOINT=https://otlp.eu01.nr-data.net:443
```

La clave de licencia de New Relic no debe commitearse en el repositorio. Debe guardarse como variable secreta o variable de entorno del servicio.

## Señales observadas

La estrategia inicial se enfoca en:

- Latencia de requests.
- Tráfico recibido por la API.
- Errores HTTP.
- Métricas básicas del runtime .NET.

Esta selección se alinea con los Golden Signals de observabilidad: latencia, tráfico, errores y saturación.

## Endpoints utilizados para validación

```txt
/health
/ready
/diagnostics/slow
/diagnostics/error
```

## Validacion

Para generar trafico y trazas de prueba:

```bash
curl https://devops-tp-api.onrender.com/health
curl https://devops-tp-api.onrender.com/diagnostics/slow
curl -i https://devops-tp-api.onrender.com/diagnostics/error
```

Para generar un volumen de trafico mas completo (lecturas, creacion de quests, ciclo de vida, latencia y errores) se puede usar el script:

```bash
BASE_URL=https://devops-tp-api.onrender.com ./scripts/generate-demo-traffic.sh
```

En New Relic se debe validar que aparezca el servicio `devops-tp-api` con metricas de requests, latencia, errores y runtime .NET.

## Evidencia sugerida (capturas)

Para documentar la entrega, tomar capturas de:

- APM overview del servicio `devops-tp-api`.
- Throughput (requests por minuto).
- Response time (incluyendo el pico generado por `/diagnostics/slow`).
- Errors (errores generados por `/diagnostics/error`).
- Distributed traces de un request.

Guardarlas en `docs/screenshots/`.
