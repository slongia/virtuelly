using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1. Logging (Serilog for consistent logs across services)
// ------------------------------------------------------------
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
    cfg.WriteTo.Console();
});

// ------------------------------------------------------------
// 2. Razor Pages UI
//    We add global auth filter later when you want pages protected.
// ------------------------------------------------------------
builder.Services.AddRazorPages(options =>
{
    // Example: force auth globally (commented out for now)
    // var policy = new AuthorizationPolicyBuilder()
    //     .RequireAuthenticatedUser()
    //     .Build();
    // options.Conventions.AuthorizeFolder("/Secure", policy);
});


// ------------------------------------------------------------
// 3. Cookie auth for Razor.Web itself
//    - Razor.Web needs to know "am I logged in?" to personalize UI.
//    - The cookie here is *Razor-local session state*, NOT the JWT for APIs.
// ------------------------------------------------------------
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // where to send users if they're not logged in
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";

        // secure settings you’ll harden later
        options.Cookie.Name = "virtuelly.auth";
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
    });

// You can tighten this later, e.g. [Authorize] on sensitive pages
builder.Services.AddAuthorization();

// ------------------------------------------------------------
// 4. HttpClient(s) for backend calls through ApiGateway
//
// We will call ApiGateway from Razor.Web to:
//   - register/login users (/api/auth/...)
//   - fetch profile (/api/users/...)
//   - upload content (/api/content/...)
//
// Gateway:BaseUrl should be set in Razor.Web's appsettings.json, e.g.
//   "Gateway": {
//      "BaseUrl": "https://localhost:7000"
//   }
// ------------------------------------------------------------
builder.Services.AddHttpClient("GatewayClient", (sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["Gateway:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        // dev fallback (adjust to your ApiGateway dev URL/port)
        baseUrl = "https://localhost:7000";
    }

    client.BaseAddress = new Uri(baseUrl);
    // We'll attach bearer tokens per-request in page models or a helper service,
    // not globally here, because tokens are per-user.
});

builder.Services.AddHttpClient(); // generic factory for AuthClient
builder.Services.AddScoped<Razor.Web.Services.AuthClient>();

// ------------------------------------------------------------
// 5. App build pipeline
// ------------------------------------------------------------
var app = builder.Build();

// Dev vs prod safety nets
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// HTTP -> HTTPS, static files, etc.
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();

// Routing
app.UseRouting();

// AuthN / AuthZ for Razor pages
app.UseAuthentication();
app.UseAuthorization();

// Map Razor Pages (Pages/*.cshtml + Pages/*.cshtml.cs)
app.MapRazorPages();

// Simple ping for sanity (optional)
// This does not expose secrets; it's just a "am I alive" check.
app.MapGet("/health", () => Results.Ok(new
{
    service = "Razor.Web",
    status = "ok",
    timestampUtc = DateTime.UtcNow
}));

app.Run();

