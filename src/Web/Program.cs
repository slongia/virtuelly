using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.Data;
using Infrastructure.Services;
using Infrastructure.Repositories;
using Core.Entities;
using Core.Interfaces;
using Application.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//add services
// Configure the existing Identity application cookie (Identity already registers the scheme)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SignInManager<User>>();


// Add services to the container.
builder.Services.AddRazorPages();


// Configure JWT authentication
var authBuilder = builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
});

authBuilder.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "virtual_secret_key_1234567890")) // Default key if not set
    };
    //Read token from http only cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("auth_token"))
            {
                context.Token = context.Request.Cookies["auth_token"];
            }
            return Task.CompletedTask;
        }
    };
});

authBuilder.AddGoogle(options =>
{
    options.SignInScheme = IdentityConstants.ExternalScheme; // use external temp cookie
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
    options.SaveTokens = true; // keep access/refresh tokens (if provided) in auth properties
    options.Scope.Add("profile");
    options.Scope.Add("email");
});
/*
authBuilder.AddMicrosoftAccount(options =>
{
    options.SignInScheme = IdentityConstants.ExternalScheme;
    options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"] ?? string.Empty;
    options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"] ?? string.Empty;
    options.SaveTokens = true;
    options.Scope.Add("User.Read");
});

authBuilder.AddFacebook(options =>
{
    options.SignInScheme = IdentityConstants.ExternalScheme;
    options.AppId = builder.Configuration["Authentication:Facebook:AppId"] ?? string.Empty;
    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"] ?? string.Empty;
    options.SaveTokens = true;
    options.Fields.Add("name");
    options.Fields.Add("email");
});
*/
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
