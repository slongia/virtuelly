using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Features.Auth;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthTokenResponse(string accessToken, string tokenType, int expiresInSeconds);

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _users;
    private readonly SignInManager<IdentityUser> _signIn;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<IdentityUser> users,
        SignInManager<IdentityUser> signIn,
        IConfiguration config)
    {
        _users = users;
        _signIn = signIn;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        var user = new IdentityUser
        {
            UserName = req.Email,
            Email = req.Email
        };

        var result = await _users.CreateAsync(user, req.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { errors });
        }

        // Optionally assign default roles here, etc.

        var token = await GenerateJwt(user);
        return Ok(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _users.FindByEmailAsync(req.Email);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var pwCheck = await _signIn.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: true);
        if (!pwCheck.Succeeded)
            return Unauthorized("Invalid credentials");

    var token = await GenerateJwt(user);
    return Ok(token);
    }

    private async Task<AuthTokenResponse> GenerateJwt(IdentityUser user)
    {
        var issuer = _config["Jwt:Issuer"];
        var audience = _config["Jwt:Audience"];
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:SigningKey"]!)
        );

        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new("uid", user.Id)
        };

        // include roles in JWT 
        var roles = await _users.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role", role));
        }
        var expires = DateTime.UtcNow.AddMinutes(15);

        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new AuthTokenResponse(
            accessToken: encodedJwt,
            tokenType: "Bearer",
            expiresInSeconds: (int)TimeSpan.FromMinutes(15).TotalSeconds
        );
    }
}
