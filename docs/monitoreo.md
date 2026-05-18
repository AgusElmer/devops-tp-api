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
curl https://tp-devops-g3-2026.onrender.com/health
curl https://tp-devops-g3-2026.onrender.com/diagnostics/slow
curl -i https://tp-devops-g3-2026.onrender.com/diagnostics/error
```

En New Relic se debe validar que aparezca el servicio `devops-tp-api` con metricas de requests, latencia, errores y runtime .NET.
