using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using DevOpsTp.Api.Quests;

var builder = WebApplication.CreateBuilder(args);

var serviceName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") ?? "devops-tp-api";

var serviceVersion = Environment.GetEnvironmentVariable("APP_VERSION")
    ?? Environment.GetEnvironmentVariable("GIT_COMMIT")
    ?? "1.0.0";

var environmentName = builder.Environment.EnvironmentName;

var otlpEndpointConfigured = !string.IsNullOrWhiteSpace(
    Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT")
);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(
            serviceName: serviceName,
            serviceVersion: serviceVersion)
        .AddAttributes(new[]
        {
            new KeyValuePair<string, object>("deployment.environment", environmentName)
        }))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true;
            })
            .AddHttpClientInstrumentation();

        if (otlpEndpointConfigured)
        {
            tracing.AddOtlpExporter();
        }
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation();

        if (otlpEndpointConfigured)
        {
            metrics.AddOtlpExporter();
        }
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<QuestStore>();

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new
        {
            title = "An unexpected error occurred",
            status = StatusCodes.Status500InternalServerError,
            traceId = context.TraceIdentifier
        });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", (IWebHostEnvironment environment) =>
{
    return Results.Ok(new
    {
        app = "DevOps TP API",
        description = "API skeleton for a DevOps practical project",
        status = "running",
        environment = environment.EnvironmentName,
        version = ApiVersion.Current,
        timestamp = DateTimeOffset.UtcNow
    });
})
.WithName("GetApiInfo")
.WithTags("General");

app.MapHealthChecks("/health")
.WithName("HealthCheck")
.WithTags("Diagnostics");

app.MapGet("/ready", () =>
{
    return Results.Ok(new
    {
        status = "ready",
        message = "The API is ready to receive traffic",
        timestamp = DateTimeOffset.UtcNow
    });
})
.WithName("ReadinessCheck")
.WithTags("Diagnostics");

app.MapGet("/version", () =>
{
    return Results.Ok(new
    {
        version = ApiVersion.Current,
        commit = Environment.GetEnvironmentVariable("GIT_COMMIT") ?? "local",
        buildDate = Environment.GetEnvironmentVariable("BUILD_DATE") ?? "local"
    });
})
.WithName("GetVersion")
.WithTags("General");

app.MapGet("/diagnostics/ping", () =>
{
    return Results.Ok(new
    {
        message = "pong",
        timestamp = DateTimeOffset.UtcNow
    });
})
.WithName("Ping")
.WithTags("Diagnostics");

app.MapGet("/diagnostics/error", () =>
{
    throw new InvalidOperationException("Controlled error for monitoring validation");
})
.WithName("SimulateError")
.WithTags("Diagnostics");

app.MapGet("/diagnostics/slow", async () =>
{
    await Task.Delay(2000);

    return Results.Ok(new
    {
        message = "Slow response completed",
        delayMilliseconds = 2000,
        timestamp = DateTimeOffset.UtcNow
    });
})
.WithName("SimulateSlowRequest")
.WithTags("Diagnostics");

app.MapQuestEndpoints();

app.Run();

public static class ApiVersion
{
    public const string Current = "1.0.0";
}

public partial class Program { }
