namespace ParkG.Infrastructure.Security;

public class JwtOptions
{
    public string Issuer { get; set; } = "ParkG";
    public string Audience { get; set; } = "ParkG";
    public string SigningKey { get; set; } = "DEV_ONLY_CHANGE_ME_32CHAR_MINIMUM_KEY_123";
    public int ExpiryMinutes { get; set; } = 120;
}