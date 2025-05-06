using System.ComponentModel.DataAnnotations;

namespace TaskSystem.Domain.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(64)]
    public string UserName { get; set; } = default!;

    [MaxLength(256)]
    public string PasswordHash { get; set; } = default!;

    [MaxLength(32)]
    public string Role { get; set; } = "User";
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

}