using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi.Configurations;
using WebApi.Interfaces;
using WebApi.Middlewares;
using DefaultHttpContext = Microsoft.AspNetCore.Http.DefaultHttpContext;

namespace UnitTests.WebApi.Middlewares;

public class JwtRenewalMiddlewareTest
{
    private readonly JwtConfig _jwtConfig;
    
    public JwtRenewalMiddlewareTest()
    {
        _jwtConfig = new JwtConfig
        {
            Secret = "RGV1cyBzZWphIGxvdXZhZG8gZSBub3NzbyBTZW5ob3IgSmVzdXMgc2VqYSBleGFsdGFkbw==",
            ValidIssuer = "http://localhost:5000",
            ValidAudience = "http://localhost:5000",
            ExpireMinutes = 30,
            RenewNearExpirationMinutes = 5
        };
    }
    
    [Fact]
    public async Task JwtRenewalMiddleware_Should_Renew_Token_If_Expiration_Time_Is_Near()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = $"Bearer {GenerateToken(DateTime.UtcNow.AddMinutes(4))}";
        var jwtServiceMock = new Mock<IJwtService>();
        var middleware = new JwtRenewalMiddleware(context => Task.CompletedTask, jwtServiceMock.Object, _jwtConfig);
        
        // Act
        await middleware.InvokeAsync(httpContext);

        // Assert
        jwtServiceMock.Verify(s => s.RenewToken(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task JwtRenewalMiddleware_Should_Not_Renew_Token_If_Expiration_Time_Is_Far()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Authorization"] = $"Bearer {GenerateToken(DateTime.UtcNow.AddMinutes(30))}";
        var jwtServiceMock = new Mock<IJwtService>();
        var middleware = new JwtRenewalMiddleware(context => Task.CompletedTask, jwtServiceMock.Object, _jwtConfig);
        
        // Act
        await middleware.InvokeAsync(httpContext);
    
        // Assert
        jwtServiceMock.Verify(s => s.RenewToken(It.IsAny<string>()), Times.Never);
    }
    
    private string GenerateToken(DateTimeOffset expirationTime)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = expirationTime.UtcDateTime,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
    
        return tokenHandler.WriteToken(token);
    }
}