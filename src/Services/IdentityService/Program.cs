
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// required so EF tooling can resolve services later if needed:
builder.Services.AddDbContext<PlaceholderContext>();

builder.Services.AddControllers();

var app = builder.Build();
app.MapGet("/", () => "IdentityService placeholder");
app.Run();

// temporary DbContext just so build succeeds;
// EF will overwrite with the real scaffolded context
public class PlaceholderContext : Microsoft.EntityFrameworkCore.DbContext
{
    public PlaceholderContext(DbContextOptions<PlaceholderContext> options)
        : base(options)
    {
    }
}
































// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.OpenApi.Models;
// using Serilog;

// var builder = WebApplication.CreateBuilder(args);

// // Serilog (basic console logging for now)
// builder.Host.UseSerilog((ctx, cfg) =>
// {
//     cfg.ReadFrom.Configuration(ctx.Configuration);
//     cfg.WriteTo.Console();
// });

// // Add controllers
// builder.Services.AddControllers();

// // Add Swagger
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen(o =>
// {
//     o.SwaggerDoc("v1", new OpenApiInfo { Title = "Service API", Version = "v1" });
//     // Allow "Authorize: Bearer <token>" in Swagger UI
//     var jwtScheme = new OpenApiSecurityScheme
//     {
//         Name = "Authorization",
//         Type = SecuritySchemeType.Http,
//         Scheme = "bearer",
//         BearerFormat = "JWT",
//         In = ParameterLocation.Header,
//         Description = "JWT Authorization header using the Bearer scheme."
//     };
//     o.AddSecurityDefinition("Bearer", jwtScheme);
//     o.AddSecurityRequirement(new OpenApiSecurityRequirement {
//         { jwtScheme, Array.Empty<string>() }
//     });
// });

// // Add health checks
// builder.Services.AddHealthChecks();

// // Add auth (JWT bearer)
// builder.Services
//     .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.Authority = builder.Configuration["Jwt:Authority"];   // e.g. https://identity.local
//         options.Audience = builder.Configuration["Jwt:Audience"];     // e.g. api
//         options.RequireHttpsMetadata = false; // dev only
//     });

// builder.Services.AddAuthorization();

// // TODO: Add EF Core DbContext for this service's schema
// // builder.Services.AddDbContext<YourContext>(...);

// // TODO: Add any service-specific services (NotificationSender, ProfileRepository, etc.)

// var app = builder.Build();

// // dev-only swagger
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

// app.UseSerilogRequestLogging();

// app.UseHttpsRedirection();

// // authN/authZ
// app.UseAuthentication();
// app.UseAuthorization();

// app.MapControllers();
// app.MapHealthChecks("/health");

// app.Run();
