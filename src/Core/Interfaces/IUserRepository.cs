using Core.Entities;
using System.Threading.Tasks;
namespace Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task AddRoleAsync(User user, string role);
}