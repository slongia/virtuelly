using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Web.Services;

namespace Razor.Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly AuthClient _authClient;

    public RegisterModel(AuthClient authClient)
    {
        _authClient = authClient;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // just render form
    }

    public async Task<IActionResult> OnPostAsync()
    {

        // foreach (var kvp in ModelState)
        // {
        //     var key = kvp.Key;
        //     var state = kvp.Value;
        //     foreach (var err in state.Errors)
        //     {
        //         Console.WriteLine($"MODEL ERROR: {key} => {err.ErrorMessage}");
        //     }
        // }

        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fix validation errors.";
            return Page();
        }

        var (ok, error, token) = await _authClient.RegisterAsync(Input.Email, Input.Password);
        if (!ok || token is null)
        {
            ErrorMessage = error ?? "Registration failed.";
            return Page();
        }

        // save the token in the auth cookie so Razor.Web knows you're logged in
        await SignInWithToken(token, Input.Email);

        // redirect to a logged-in page (dashboard/home etc.)
        return RedirectToPage("/Index");
    }

    private async Task SignInWithToken(AuthTokenResponse token, string email)
    {
        // We store minimal identity in the cookie for Razor.Web UI personalization.
        // The real API calls will still send the bearer token later when calling Gateway.
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim("accessToken", token.accessToken)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true, // Keep login across browser sessions (up to cookie lifetime)
                AllowRefresh = true
            });
    }

    private async Task<HttpClient> CreateAuthedGatewayClient()
    {
        var accessToken = User.FindFirst("accessToken")?.Value;
        var client = HttpContext.RequestServices
            .GetRequiredService<IHttpClientFactory>()
            .CreateClient();

        var config = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var baseUrl = config["Gateway:BaseUrl"] ?? "https://localhost:7000";
        client.BaseAddress = new Uri(baseUrl);

        if (!string.IsNullOrEmpty(accessToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }

        return client;
    }


    public class RegisterInput
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(8)]
        public string Password { get; set; } = "";
    }
}
