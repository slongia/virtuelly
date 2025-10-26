using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using IdentityService.Features.User.Data;


var builder = WebApplication.CreateBuilder(args);

// 1. Logging (Serilog)
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
    cfg.WriteTo.Console();
});

// 2. Add controllers (API endpoints)
builder.Services.AddControllers();

// 3. Add EF Core DbContext for the identity schema
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    var connString = builder.Configuration.GetConnectionString("Postgres");

    options.UseNpgsql(connString, npgsql =>
    {
        // optional: map migrations history table to identity schema if you ever add migrations
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
    });
});

// 4. Authentication & Authorization with JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // For now we just set validation parameters inline.
        // Later we’ll sign tokens here and also validate Issuer/Audience.
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!)
            ),

            ValidateLifetime = true
        };

        options.RequireHttpsMetadata = false; // dev only
        options.SaveToken = true;
    });

builder.Services.AddAuthorization();

// 5. Swagger (with Bearer button)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityService", Version = "v1" });

    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    };

    o.AddSecurityDefinition("Bearer", jwtScheme);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { jwtScheme, Array.Empty<string>() }
    });
});

// 6. Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Swagger UI in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

// auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
