using Core.Entities.Result;
using System.Threading.Tasks;
namespace Core.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string email, string password, string fullName);
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> ExternalLoginAsync(string provider, string email, string fullName);
    Task<AuthResult> SocialLoginAsync(string provider, string token);
}
