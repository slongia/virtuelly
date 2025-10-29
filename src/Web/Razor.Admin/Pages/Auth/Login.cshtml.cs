using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Admin.Services;

namespace Razor.Admin.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly AuthClientAdmin _authClient;

    public LoginModel(AuthClientAdmin authClient)
    {
        _authClient = authClient;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // just render the login form
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fix validation errors.";
            return Page();
        }

        // Ask IdentityService for a token
        var (ok, error, token) =
            await _authClient.LoginAsync(Input.Email, Input.Password);

        if (!ok || token is null)
        {
            ErrorMessage = error ?? "Login failed.";
            return Page();
        }

        // Decode JWT to inspect roles
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token.accessToken);

        // We expect "Admin" role in token
        var roles = jwt.Claims
            .Where(c =>
                c.Type == ClaimTypes.Role ||
                c.Type == "role")
            .Select(c => c.Value)
            .Distinct()
            .ToList();

        var isAdmin = roles.Contains("Admin", StringComparer.OrdinalIgnoreCase);

        if (!isAdmin)
        {
            ErrorMessage = "You are not authorized for the admin portal.";
            return Page();
        }

        // Build local auth cookie for Razor.Admin
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, Input.Email),
            // keep the accessToken so AdminUsersClient can call ApiGateway with it
            new Claim("accessToken", token.accessToken),
            // also persist roles locally for [Authorize(Roles="Admin")] in Razor.Admin
            new Claim(ClaimTypes.Role, "Admin")
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
                IsPersistent = true,
                AllowRefresh = true
            });

        // Go to the users list
        return RedirectToPage("/Admin/Users/Index");
    }

    public class LoginInput
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(8)]
        public string Password { get; set; } = "";
    }
}
