using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using Yarp.ReverseProxy;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------
// 1. Logging via Serilog
// --------------------------------------
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
    cfg.WriteTo.Console();
});

// --------------------------------------
// 2. YARP Reverse Proxy
//    Load routes and clusters from configuration
// --------------------------------------
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// --------------------------------------
// 3. CORS for local dev
//    Allows Razor.Web, Angular.Web, etc. to hit the gateway
// --------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("dev-cors", policy =>
    {
        policy
        //.AllowAnyHeader()
        //.AllowAnyMethod()
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
            .AllowCredentials()
            .WithOrigins(
                "https://localhost:7000", // gateway itself (sometimes iframes, etc.)
                "http://localhost:7001",
                "https://localhost:5001", // Razor.Web HTTPS dev (you'll set in Razor.Web launch later)
                "http://localhost:5001",
                "http://localhost:4200",  // Angular.Web dev
                "http://localhost:4300"   // Angular.Admin dev
            );
    });
});

// --------------------------------------
// 4. JWT validation
//    The gateway is the bouncer. If the token is bad, request stops here.
// --------------------------------------
var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!)
);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Validate tokens that IdentityService mints
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateLifetime = true
        };

        options.RequireHttpsMetadata = false; // dev
        options.SaveToken = true;
    });

builder.Services.AddAuthorization();

// --------------------------------------
// 5. Swagger (optional, usually just for diagnostics at the gateway)
// --------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiGateway", Version = "v1" });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// --------------------------------------
// 6. Pipeline
// --------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("dev-cors");

// Order matters: auth BEFORE proxy
app.UseAuthentication();
app.UseAuthorization();

// /health endpoint for kubernetes / compose / uptime checks
app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "ApiGateway" })).RequireCors("dev-cors");;

// ReverseProxy will now handle all /api/... calls
app.MapReverseProxy().RequireCors("dev-cors");

app.Run();
