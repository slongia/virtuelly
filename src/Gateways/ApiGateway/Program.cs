using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// logging
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
    cfg.WriteTo.Console();
});

// reverse proxy (routes, clusters come from yarp.routes.json / appsettings.json)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// swagger (optional: doc for gateway endpoints if you expose any)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiGateway", Version = "v1" });
});

// CORS for local Angular/Razor dev
builder.Services.AddCors(options =>
{
    options.AddPolicy("dev-cors", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins(
                  "http://localhost:4200", // Angular.Web
                  "http://localhost:4300", // Angular.Admin
                  "https://localhost:5001", // Razor.Web dev https
                  "https://localhost:5002"  // Razor.Admin dev https
              );
    });
});

// JWT validation (gateway enforces auth before proxying)
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"];
        options.Audience = builder.Configuration["Jwt:Audience"];
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();

// health check: gateway is alive
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseCors("dev-cors");

// all requests go through auth unless you mark certain routes as anonymous
app.UseAuthentication();
app.UseAuthorization();

// mount reverse proxy
app.MapReverseProxy();

// health
app.MapGet("/health", () => Results.Ok("ok"));

app.Run();
