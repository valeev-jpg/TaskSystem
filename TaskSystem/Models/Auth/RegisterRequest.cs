namespace TaskSystem.Models.Auth;

public record RegisterRequest(string UserName, string Password, string Role = "User");

