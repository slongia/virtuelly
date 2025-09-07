using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
namespace Virtuelly.Web.Pages.Account;

public class ExternalLoginModel : PageModel
{
    private readonly UserService _userService;

    public ExternalLoginModel(UserService userService)
    {
        _userService = userService;
    }


    public async Task<IActionResult> OnGetCallbackAsync()
    {

        var info = await HttpContext.AuthenticateAsync("Cookies");
        if (info?.Principal == null)
        {
            return RedirectToPage("./Login");
        }
        var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        var fullName = info.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var provider = info.Properties?.Items[".AuthScheme"] ?? string.Empty;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(provider))
        {
            return RedirectToPage("./Login");
        }


        // Get the external login info (in a real app, you'd get this from the external provider)
        //var email = "user@example.com";

        // Use the UserService to sign in the user
        var result = await _userService.ExternalLoginAsync(provider, email, fullName);
        if (result.Success)
        {
            // Set JWT in HttpOnly cookie
            Response.Cookies.Append("auth_token", result.Token!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
            return RedirectToPage("/Index");
        }
        // If login failed, show an error message
        ModelState.AddModelError(string.Empty, "External login failed.");
        return Page();
    }


    public void OnGet()
    {
    }
}

