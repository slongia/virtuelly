using Core.Entities;
namespace Core.Interfaces;

public interface ITokenService
{
    string GenerateJwtToken(User user, IList<string> roles);
}