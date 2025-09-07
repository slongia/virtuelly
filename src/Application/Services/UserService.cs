using Application.Dtos;
using Application.Mappings;
using Core.Entities.Result;
using Core.Interfaces;
using System.Threading.Tasks;

namespace Application.Services;

public class UserService
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;

    public UserService(IAuthService authService, IUserRepository userRepository, ITokenService tokenService)
    {
        _authService = authService;
        _userRepository = userRepository;
        _tokenService = tokenService;
    }

    public async Task<AuthResultDto> RegisterAsync(string email, string password, string fullName)
    {
        var authResult = await _authService.RegisterAsync(email, password, fullName);
        if (!authResult.Success)
        {
            return new AuthResultDto { Success = false, Errors = authResult.Errors };
        }
        var user = authResult.User!;
        await _userRepository.AddRoleAsync(user, "User");
        var roles = new List<string> { "User" };
        var token = _tokenService.GenerateJwtToken(user, roles);
        return new AuthResultDto { Success = true, Token = token };
    }

    public async Task<AuthResultDto> LoginAsync(string email, string password)
    {
        var authResult = await _authService.LoginAsync(email, password);
        if (!authResult.Success)
        {
            return new AuthResultDto { Success = false, Errors = authResult.Errors };
        }
        var user = authResult.User!;
        //var roles = await _userRepository.GetRolesAsync(user);
        var roles = new List<string> { "User" };  // Fetch real roles if needed
        var token = _tokenService.GenerateJwtToken(user, roles);
        return new AuthResultDto { Success = true, Token = token };
    }

    public async Task<AuthResultDto> ExternalLoginAsync(string provider, string email, string fullName)
    {
        var authResult = await _authService.ExternalLoginAsync(provider, email, fullName);
        if (!authResult.Success)
        {
            return new AuthResultDto { Success = false, Errors = authResult.Errors };
        }
        var user = authResult.User!;
        await _userRepository.AddRoleAsync(user, "User");
        var roles = new List<string> { "User" };  // Fetch real roles if needed
        var token = _tokenService.GenerateJwtToken(user, roles);
        return new AuthResultDto { Success = true, Token = token };
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null ? UserMapper.ToDto(user) : null;
    }

}
