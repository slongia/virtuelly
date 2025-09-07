using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task AddRoleAsync(User user, string role)
    {
        var identityRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);
        if (identityRole == null)
        {
            identityRole = new IdentityRole<Guid> { Name = role, NormalizedName = role.ToUpper() };
            _context.Roles.Add(identityRole);
            await _context.SaveChangesAsync();
        }

        var userRole = new IdentityUserRole<Guid> { UserId = user.Id, RoleId = identityRole.Id };
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}