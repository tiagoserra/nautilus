using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Configurations;
using WebApi.Interfaces;

namespace WebApi.Services;

public class JwtService : IJwtService
{
    private readonly JwtConfig _jwtConfig;

    public JwtService(IOptionsMonitor<JwtConfig> optionsMonitor)
        => _jwtConfig = optionsMonitor.CurrentValue;
    
    public string GenerateToken(Guid uniqueIdentifier, string login, string email, string userId, string[] roles,
        string[] policies = null)
    {
        List<Claim> claims = new()
        {
            new Claim(JwtRegisteredClaimNames.Sub, uniqueIdentifier.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, login),
            new Claim("user_id", userId),
            new Claim(type: "SessionIdentifier", value: Guid.NewGuid().ToString())
        };

        if (roles != null)
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

        if (policies != null)
            foreach (var policy in policies)
                claims.Add(new Claim("policy", policy));

        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtConfig.ValidIssuer,
            Audience = _jwtConfig.ValidAudience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpireMinutes),
            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string RenewToken(string expiredToken)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(expiredToken);
        var newTokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtConfig.ValidIssuer,
            Audience = _jwtConfig.ValidAudience,
            Subject = new ClaimsIdentity(jwtToken.Subject),
            Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.ExpireMinutes),
            SigningCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256Signature)
        };

        var newToken = tokenHandler.CreateJwtSecurityToken(newTokenDescriptor);
        return tokenHandler.WriteToken(newToken);
    }
}