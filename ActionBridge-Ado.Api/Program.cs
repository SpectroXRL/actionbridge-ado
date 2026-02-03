using ActionBridge_Ado.Api;
using ActionBridge_Ado.Api.Endpoints;
using ActionBridge_Ado.Api.Services.Ado;
using ActionBridge_Ado.Api.Services.AI;
using ActionBridge_Ado.Api.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<IAdoService, AdoService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReactApp");

app.MapFileEndpoints();
app.MapAdoEndpoints();

app.Run();
