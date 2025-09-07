using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Virtuelly.Web.Pages;

[Authorize]
public class IndexModel : PageModel
{
    public void OnGet()
    {

    }
}
