namespace TaskSystem.Models.Auth;

public class JwtSettings
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Secret { get; set; } = default!;
    public int AccessExpiryMinutes { get; set; } = 60;
    public int RefreshExpiryDays { get; set; } = 7;
}