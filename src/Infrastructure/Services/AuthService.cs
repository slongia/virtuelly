using Microsoft.AspNetCore.Identity;
using Core.Entities;
using Core.Interfaces;
using System.Threading.Tasks;
using Core.Entities.Result;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.CompilerServices;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserRepository _userRepository;

    public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IUserRepository userRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userRepository = userRepository;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string fullName)
    {
        var user = new User
        {
            UserName = email,
            Email = email,
            FullName = fullName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return new AuthResult
            {
                Success = true,
                User = user
            };
        }
        return new AuthResult
        {
            Success = false,
            Errors = result.Errors.Select(e => e.Description)
        };
    }
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return new AuthResult { Success = false, Errors = new[] { "User not found" } };
        }

        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (result.Succeeded)
        {
            return new AuthResult { Success = true, User = user };
        }

        return new AuthResult { Success = false, Errors = new[] { "Invalid login attempt" } };
    }


    public async Task<AuthResult> ExternalLoginAsync(string provider, string email, string fullName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User { UserName = email, Email = email, FullName = fullName };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return new AuthResult { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }
        }
        return new AuthResult { Success = true, User = user };
    }





    public Task<AuthResult> SocialLoginAsync(string provider, string token)
    {
        throw new NotImplementedException();
    }
}