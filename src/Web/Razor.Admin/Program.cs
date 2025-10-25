using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// logging
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
    cfg.WriteTo.Console();
});

// Razor Pages
builder.Services.AddRazorPages();

// Cookie auth for Razor session
builder.Services
    .AddAuthentication(CookiesAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        // you can map these pages
    });

// Authorization for [Authorize] on pages
builder.Services.AddAuthorization();

// HttpClient to call ApiGateway on behalf of the signed-in user
builder.Services.AddHttpClient("ApiGatewayClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Gateway:BaseUrl"] ?? "https://localhost:7000");
    // You’ll forward user JWT or service credentials when calling downstream
});

var app = builder.Build();

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

app.MapRazorPages();

app.Run();
