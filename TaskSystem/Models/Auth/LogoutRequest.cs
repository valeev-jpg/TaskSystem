namespace TaskSystem.Models.Auth;

public record LogoutRequest(string RefreshToken, bool All = false);
