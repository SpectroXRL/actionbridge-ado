using ActionBridge_Ado.Api;
using ActionBridge_Ado.Api.Endpoints;
using ActionBridge_Ado.Api.Models;
using ActionBridge_Ado.Api.Services.Ado;
using ActionBridge_Ado.Api.Services.AI;
using ActionBridge_Ado.Api.Services.File;
using ActionBridge_Ado.Api.Services.Chunker;
using ActionBridge_Ado.Api.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<AzureOpenAIOptions>(builder.Configuration.GetSection("AzureOpenAI"));

builder.Services.AddOpenApi();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<ITranscriptChunker, TranscriptChunker>();
builder.Services.AddScoped<IAdoService, AdoService>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHealthChecks();

if (builder.Environment.IsProduction())
{
    var OtlpEndpoint = builder.Configuration["Grafana:OtlpEndpoint"];
    builder.Logging.AddOpenTelemetry(options =>
    {
        options
            .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri($"{OtlpEndpoint}/v1/logs");
                    options.Headers = builder.Configuration["Grafana:OtlpHeaders"];
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                });
    });
    builder.Services.AddOpenTelemetry()
          .ConfigureResource(resource => resource.AddService(builder.Configuration["Grafana:ServiceName"]!))
          .WithTracing(tracing => tracing
              .AddAspNetCoreInstrumentation()
              .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri($"{OtlpEndpoint}/v1/traces");
                    options.Headers = builder.Configuration["Grafana:OtlpHeaders"];
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                }))
          .WithMetrics(metrics => metrics
              .AddAspNetCoreInstrumentation()
              .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri($"{OtlpEndpoint}/v1/metrics");
                    options.Headers = builder.Configuration["Grafana:OtlpHeaders"];
                    options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                }));
}
else
{
    var OtlpEndpoint = builder.Configuration["Grafana:OtlpEndpoint"];
    builder.Logging.AddOpenTelemetry(options =>
    {
        options
            .AddConsoleExporter();
    });
    // builder.Services.AddOpenTelemetry()
    //       .ConfigureResource(resource => resource.AddService(builder.Configuration["Grafana:ServiceName"]!))
    //       .WithTracing(tracing => tracing
    //           .AddAspNetCoreInstrumentation()
    //           .AddConsoleExporter())
    //       .WithMetrics(metrics => metrics
    //           .AddAspNetCoreInstrumentation()
    //           .AddConsoleExporter());
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapFileEndpoints();
app.MapAdoEndpoints();
app.MapHealthChecks("/health");

app.Run();
