using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Web.Services;

namespace Razor.Web.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly AuthClient _authClient;

    public LoginModel(AuthClient authClient)
    {
        _authClient = authClient;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        // render form
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fix validation errors.";
            return Page();
        }

        var (ok, error, token) = await _authClient.LoginAsync(Input.Email, Input.Password);
        if (!ok || token is null)
        {
            ErrorMessage = error ?? "Login failed.";
            return Page();
        }

        await SignInWithToken(token, Input.Email);

        return RedirectToPage("/Index");
    }

    private async Task SignInWithToken(AuthTokenResponse token, string email)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim("accessToken", token.accessToken)
            //,
            //new Claim("tokenExpiresIn", token.expiresIn.ToString()
            //)
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
    }

    public class LoginInput
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}
