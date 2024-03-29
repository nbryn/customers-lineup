using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CLup.Domain.Users;
using Microsoft.IdentityModel.Tokens;

namespace CLup.Application.Auth;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly AppSettings _appSettings;

    public JwtTokenService(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_appSettings.JwtSecretKey));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.UserData.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("role", "User"),
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
