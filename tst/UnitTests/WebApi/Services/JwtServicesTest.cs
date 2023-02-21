using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApi.Configurations;
using WebApi.Services;

namespace UnitTests.WebApi.Services;

public class JwtServicesTest
{
    private readonly JwtConfig _jwtConfig;
    private readonly JwtService _jwtService;

    public JwtServicesTest()
    {
        _jwtConfig = new JwtConfig
        {
            Secret = "RGV1cyBzZWphIGxvdXZhZG8gZSBub3NzbyBTZW5ob3IgSmVzdXMgc2VqYSBleGFsdGFkbw==",
            ValidIssuer = "http://localhost:5000",
            ValidAudience = "http://localhost:5000",
            ExpireMinutes = 30,
            RenewNearExpirationMinutes = 5
        };
        
        var options = Options.Create(_jwtConfig);
        var optionsMonitor = new Mock<IOptionsMonitor<JwtConfig>>();
        optionsMonitor.Setup(x => x.CurrentValue).Returns(options.Value);

        _jwtService = new JwtService(optionsMonitor.Object);
    }
    
    [Fact]
    public void GenerateToken_Returns_Valid_Token()
    {
        var uniqueIdentifier = Guid.NewGuid();
        var login = "john.doe";
        var email = "john.doe@example.com";
        var userId = "123";
        var roles = new[] { "admin", "user" };
        var policies = new[] { "policy1", "policy2" };

        var token = _jwtService.GenerateToken(uniqueIdentifier, login, email, userId, roles, policies);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = _jwtConfig.ValidIssuer,
            ValidAudience = _jwtConfig.ValidAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ValidateLifetime = true,
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var identity = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

        // bug "sub" =>  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        Assert.Equal(uniqueIdentifier.ToString(),
            identity.FindFirst(JwtRegisteredClaimNames.Sub) != null
                ? identity.FindFirst(JwtRegisteredClaimNames.Sub)!.Value
                : identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")!.Value);

        // bug "email" =>  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
        Assert.Equal(email,
            identity.FindFirst(JwtRegisteredClaimNames.Email) != null
                ? identity.FindFirst(JwtRegisteredClaimNames.Email)!.Value
                : identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")!.Value);
        
        Assert.Equal(login, identity.FindFirst(JwtRegisteredClaimNames.Name)!.Value);
        Assert.Equal(userId, identity.FindFirst("user_id")!.Value);
        Assert.NotNull(identity.FindFirst("SessionIdentifier")!.Value);

        var roleClaims = identity.FindAll(ClaimTypes.Role).ToList();
        Assert.Equal(2, roleClaims.Count);
        Assert.Contains("admin", roleClaims.Select(c => c.Value));
        Assert.Contains("user", roleClaims.Select(c => c.Value));

        var policyClaims = identity.FindAll("policy").ToList();
        Assert.Equal(2, policyClaims.Count);
        Assert.Contains("policy1", policyClaims.Select(c => c.Value));
        Assert.Contains("policy2", policyClaims.Select(c => c.Value));
    }

    [Fact]
    public void RenewToken_Returns_New_Token()
    {
        // Arrange
        var uniqueIdentifier = Guid.NewGuid();
        var email = "test@test.com";
        var login = "test";
        var userId = "test_id";
        var roles = new string[] { "admin", "user" };
        var policies = new string[] { "can_read", "can_write" };
        var token = _jwtService.GenerateToken(uniqueIdentifier, login, email, userId, roles, policies);

        // Act
        var renewedToken = _jwtService.RenewToken(token);

        // Assert
        Assert.NotNull(renewedToken);
        Assert.NotEqual(token, renewedToken);
    }
}