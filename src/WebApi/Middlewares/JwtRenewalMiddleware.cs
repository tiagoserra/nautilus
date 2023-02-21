using System.IdentityModel.Tokens.Jwt;
using WebApi.Configurations;
using WebApi.Interfaces;

namespace WebApi.Middlewares;

public class JwtRenewalMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtService _jwtService;
    private readonly JwtConfig _jwtConfig;

    public JwtRenewalMiddleware(RequestDelegate next, IJwtService jwtService, JwtConfig jwtConfig)
    {
        _next = next;
        _jwtService = jwtService;
        _jwtConfig = jwtConfig;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var jwtToken = new JwtSecurityToken(token);
        var exp = jwtToken.Payload.Exp;

        if (exp.HasValue)
        {
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp.Value);
            if (expirationTime.Subtract(TimeSpan.FromMinutes(_jwtConfig.RenewNearExpirationMinutes)) < DateTimeOffset.UtcNow)
            {
                var newToken = _jwtService.RenewToken(token);

                if (!string.IsNullOrEmpty(newToken))
                    context.Request.Headers["Authorization"] = $"Bearer {newToken}";
            }
        }

        await _next(context);
    }
}