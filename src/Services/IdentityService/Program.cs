using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using IdentityService.Features.Users.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Serilog logging
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
    cfg.WriteTo.Console();
});

// 2. EF Core DbContext -> PostgreSQL
//    IMPORTANT: register IdentityAppDbContext here
builder.Services.AddDbContext<IdentityAppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Postgres"),
        npgsql =>
        {
            // Put EF migrations table in the same schema
            npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity");
        }
    );
});

// 3. ASP.NET Core Identity using IdentityAppDbContext
builder.Services
    .AddIdentityCore<IdentityUser>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityAppDbContext>() // <-- use IdentityAppDbContext
    .AddSignInManager<SignInManager<IdentityUser>>();

// 4. JWT issuance + validation
var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SigningKey"]!)
);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

// 5. Controllers + Swagger
builder.Services.AddControllers();

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

builder.Services.AddHealthChecks();

var app = builder.Build();

// middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
