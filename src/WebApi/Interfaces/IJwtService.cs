namespace WebApi.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid uniqueIdentifier, string login, string email, string userId, string[] roles,
        string[] policies = null);

    string RenewToken(string expiredToken);
}