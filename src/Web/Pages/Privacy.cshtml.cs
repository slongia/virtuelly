using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Virtuelly.Web.Pages;

//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PrivacyModel : PageModel
{
    public void OnGet()
    {
    }
}

