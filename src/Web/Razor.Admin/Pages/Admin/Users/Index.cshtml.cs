using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor.Admin.Services;

namespace Razor.Admin.Pages.Admin.Users;

public class IndexModel : PageModel
{
    private readonly AdminUsersClient _usersClient;

    public IndexModel(AdminUsersClient usersClient)
    {
        _usersClient = usersClient;
    }

    // Data for the page to render
    public PagedResult<AdminUserListItemDto>? Result { get; private set; }

    // Preserve current list state in query (paging, sorting, search)
    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 20;

    [BindProperty(SupportsGet = true)]
    public string? SortBy { get; set; } = "Email";

    [BindProperty(SupportsGet = true)]
    public string? SortDir { get; set; } = "asc";

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        // Fetch from API through gateway
        Result = await _usersClient.GetUsersAsync(
            PageNumber,
            PageSize,
            SortBy,
            SortDir,
            Search
        );

        return Page();
    }
}
