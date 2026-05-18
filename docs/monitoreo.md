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