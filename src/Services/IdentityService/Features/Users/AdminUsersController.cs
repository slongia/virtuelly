using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityService.Features.Users;
using IdentityService.Features.Users.Data;

namespace IdentityService.Features.Users;

[ApiController]
[Route("users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController : ControllerBase
{
    private readonly IdentityAppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public AdminUsersController(IdentityAppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    // GET /users?pageNumber=1&pageSize=20&sortBy=Email&sortDir=asc&search=foo
    [HttpGet]
    public async Task<ActionResult<PagedResult<AdminUserListItemDto>>> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "Email",
        [FromQuery] string? sortDir = "asc",
        [FromQuery] string? search = null)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        IQueryable<IdentityUser> query = _db.Users.AsQueryable();

        // optional filtering
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(u =>
                (u.Email ?? "").ToLower().Contains(s) ||
                (u.UserName ?? "").ToLower().Contains(s));
        }

        // sorting
        bool desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        query = (sortBy?.ToLower()) switch
        {
            "email" => desc ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "lockoutend" => desc ? query.OrderByDescending(u => u.LockoutEnd) : query.OrderBy(u => u.LockoutEnd),
            _ => desc ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id)
        };

        var totalCount = await query.CountAsync();

        var usersPage = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // gather roles per user (UserManager hits AspNetUserRoles/AspNetRoles)
        var resultItems = new List<AdminUserListItemDto>();
        foreach (var u in usersPage)
        {
            var roles = await _userManager.GetRolesAsync(u);

            resultItems.Add(new AdminUserListItemDto(
                Id: u.Id,
                Email: u.Email ?? "",
                EmailConfirmed: u.EmailConfirmed,
                LockoutEnabled: u.LockoutEnabled,
                LockoutEnd: u.LockoutEnd,
                AccessFailedCount: u.AccessFailedCount,
                Roles: roles.ToArray()
            ));
        }

        var result = new PagedResult<AdminUserListItemDto>(
            Items: resultItems,
            PageNumber: pageNumber,
            PageSize: pageSize,
            TotalCount: totalCount
        );

        return Ok(result);
    }

    // GET /users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<AdminUserDetailDto>> GetUserById(string id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new AdminUserDetailDto(
            Id: user.Id,
            Email: user.Email ?? "",
            EmailConfirmed: user.EmailConfirmed,
            PhoneNumber: user.PhoneNumber,
            LockoutEnabled: user.LockoutEnabled,
            LockoutEnd: user.LockoutEnd,
            AccessFailedCount: user.AccessFailedCount,
            Roles: roles.ToArray()
        ));
    }

    // POST /users
    [HttpPost]
    public async Task<ActionResult<AdminUserDetailDto>> CreateUser([FromBody] AdminUserCreateRequest req)
    {
        var user = new IdentityUser
        {
            Email = req.Email,
            UserName = req.Email,
            EmailConfirmed = req.EmailConfirmed,
            PhoneNumber = req.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        // return the created user
        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new AdminUserDetailDto(
            Id: user.Id,
            Email: user.Email ?? "",
            EmailConfirmed: user.EmailConfirmed,
            PhoneNumber: user.PhoneNumber,
            LockoutEnabled: user.LockoutEnabled,
            LockoutEnd: user.LockoutEnd,
            AccessFailedCount: user.AccessFailedCount,
            Roles: roles.ToArray()
        ));
    }

    // PUT /users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] AdminUserUpdateRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();

        user.Email = req.Email;
        user.UserName = req.Email;
        user.EmailConfirmed = req.EmailConfirmed;
        user.PhoneNumber = req.PhoneNumber;
        user.LockoutEnabled = req.LockoutEnabled;
        user.LockoutEnd = req.LockoutEnd;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null) return NotFound();

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
