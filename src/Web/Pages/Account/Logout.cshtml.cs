using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Core.Entities;
namespace Virtuelly.Web.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<User> _signInManager;

    public LogoutModel(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        //Sign out from Identity(clear any session data)
        await _signInManager.SignOutAsync();
        //remove jwt token cookie
        Response.Cookies.Delete("auth_token");
        //redirect to return url or home page
        returnUrl ??= Url.Page("/Account/Login")!;
        return Redirect(returnUrl);
    }

    public void OnGet()
    {
    }
}

