using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Application.Services;
namespace Virtuelly.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly UserService _userService;

    public LoginModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    public class InputModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; } = false;
    }
    public async Task OnGetAsync()
    {
        ExternalLogins = (await HttpContext.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>().GetAllSchemesAsync())
            .Where(s => s.DisplayName != null).ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _userService.LoginAsync(Input.Email, Input.Password);
        if (result.Success)
        {
            //Set JWT in HttpOnly cookie
            Response.Cookies.Append("auth_token", result.Token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            // Login successful, redirect to home page or return URL
            return RedirectToPage("/Index");
        }

        // Login failed, display errors
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        return Page();
    }

    public IActionResult OnGetExternalLogin(string provider)
    {
        // var redirectUrl = Url.Page("/Account/ExternalLoginCallback", pageHandler: null, values: new { returnUrl = Url.Content("~/") }, protocol: Request.Scheme);
        // var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        // return new ChallengeResult(provider, properties);
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, provider);
    }

}

