namespace WebApi.Configurations;

public class JwtConfig
{
    public string Secret { get; set; }
    public int ExpireMinutes { get; set; }
    public int RenewNearExpirationMinutes { get; set; }
    public string ValidIssuer { get; set; }
    public string ValidAudience { get; set; }
}