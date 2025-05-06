using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskSystem.Domain.Interfaces;
using TaskSystem.Domain.Models;
using TaskSystem.Models.Auth;
using LoginRequest = TaskSystem.Models.Auth.LoginRequest;
using RefreshRequest = TaskSystem.Models.Auth.RefreshRequest;
using RegisterRequest = TaskSystem.Models.Auth.RegisterRequest;

namespace TaskSystem.Controllers;

public class AuthController(IServiceDbContext db, IPasswordHasher<User> hasher, IOptions<JwtSettings> jwt, IHttpContextAccessor ctx) : BaseController
{
    [HttpPost]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest req)
    {
        if (await db.Users.AnyAsync(u => u.UserName == req.UserName))
            return Conflict("User already exists");

        var user = new User { UserName = req.UserName, Role = req.Role };   // ←
        user.PasswordHash = hasher.HashPassword(user, req.Password);

        var refresh = CreateRefreshToken(user, GetIpAddress());
        user.RefreshTokens.Add(refresh);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(BuildAuthResponse(user, refresh));
    }

    [HttpPost]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest req)
    {
        var user = await db.Users.Include(u => u.RefreshTokens)
                                  .FirstOrDefaultAsync(u => u.UserName == req.UserName);
        if (user == null)
            return Unauthorized("Invalid credentials");

        if (hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid credentials");

        var refresh = CreateRefreshToken(user, GetIpAddress());

        db.RefreshTokens.Add(refresh);
        
        await db.SaveChangesAsync();

        return Ok(BuildAuthResponse(user, refresh));
    }

    [HttpPost]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest req)
    {
        var token = await db.RefreshTokens.Include(t => t.User)
                                           .FirstOrDefaultAsync(t => t.Token == req.RefreshToken);
        if (token == null || !token.IsActive)
            return Unauthorized("Invalid refresh token");

        // revoke old & create new
        token.Revoked = DateTime.UtcNow;
        token.RevokedByIp = GetIpAddress();
        var newRefresh = CreateRefreshToken(token.User, GetIpAddress());
        token.ReplacedByToken = newRefresh.Token;
        db.RefreshTokens.Add(newRefresh);
        await db.SaveChangesAsync();

        return Ok(BuildAuthResponse(token.User, newRefresh));
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest req)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!Guid.TryParse(userId, out var uid))
            return Unauthorized();

        var user = await db.Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == uid);
        if (user == null)
            return Unauthorized();

        if (req.All)
        {
            foreach (var rt in user.RefreshTokens.Where(t => t.IsActive))
            {
                rt.Revoked = DateTime.UtcNow;
                rt.RevokedByIp = GetIpAddress();
            }
        }
        else
        {
            var token = user.RefreshTokens.FirstOrDefault(t => t.Token == req.RefreshToken);
            if (token == null || !token.IsActive)
                return NotFound("Refresh token not found or already revoked");

            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = GetIpAddress();
        }

        await db.SaveChangesAsync();
        return Ok();
    }
    
    private AuthResponse BuildAuthResponse(User user, RefreshToken refresh)
    {
        var accessExpires = DateTime.UtcNow.AddMinutes(jwt.Value.AccessExpiryMinutes);
        string accessToken = GenerateAccessToken(user, accessExpires);

        return new AuthResponse
        {
            AccessToken = accessToken,
            AccessTokenExpires = accessExpires,
            RefreshToken = refresh.Token,
            RefreshTokenExpires = refresh.Expires
        };
    }

    private string GenerateAccessToken(User user, DateTime expires)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Value.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer:   jwt.Value.Issuer,
            audience: jwt.Value.Audience,
            expires:  expires,
            signingCredentials: creds,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,  user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim("Role",           user.Role)
            });

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }


    private RefreshToken CreateRefreshToken(User user, string ip)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomBytes),
            Expires = DateTime.UtcNow.AddDays(jwt.Value.RefreshExpiryDays),
            Created = DateTime.UtcNow,
            CreatedByIp = ip,
            UserId = user.Id
        };
    }
    
    private string GetIpAddress() => ctx.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}