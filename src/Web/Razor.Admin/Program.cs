using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Razor.Admin.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Logging
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
    cfg.WriteTo.Console();
});

// 2. Razor Pages
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
});

// 3. Cookie auth (admin UI session)
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.Cookie.Name = "virtuelly.admin.auth";
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p =>
    {
        p.RequireAuthenticatedUser();
        p.RequireRole("Admin");
    });
});

// 4. HttpClient + AdminUsersClient for API calls
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AdminUsersClient>();
builder.Services.AddScoped<AuthClientAdmin>();

var app = builder.Build();

// pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Admin pages, Razor Pages endpoints
app.MapRazorPages();

// health check
app.MapGet("/health", () => Results.Ok(new
{
    service = "Razor.Admin",
    status = "ok",
    timestampUtc = DateTime.UtcNow
}));

app.Run();
