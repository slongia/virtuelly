using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application.Services;
namespace Virtuelly.Web.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserService _userService;

    public RegisterModel(UserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();


    public class InputModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _userService.RegisterAsync(Input.Email, Input.Password, Input.FullName);
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

            // Registration successful, redirect to login or home page
            return RedirectToPage("/Index");
        }

        // Registration failed, display errors
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error);
        }
        return Page();
    }


    public void OnGet()
    {
    }
}


