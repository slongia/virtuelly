using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Features.Auth;

public static class AuthTokens
{
    public static async Task<(string accessToken, DateTime expiresUtc)> CreateAccessJwtAsync(
        IConfiguration config,
        UserManager<IdentityUser> userManager,
        IdentityUser user)
    {
        var issuer = config["Jwt:Issuer"];
        var audience = config["Jwt:Audience"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SigningKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new("uid", user.Id),
        };
        foreach (var r in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
            claims.Add(new Claim("role", r));
        }

        var expiresUtc = DateTime.UtcNow.AddMinutes(15);
        var jwt = new JwtSecurityToken(issuer, audience, claims, expires: expiresUtc, signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(jwt), expiresUtc);
    }

    public static string CreateRefreshToken() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
}
