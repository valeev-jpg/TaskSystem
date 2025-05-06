namespace TaskSystem.Models.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = default!;
    public DateTime AccessTokenExpires { get; set; }
    public string RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpires { get; set; }
}