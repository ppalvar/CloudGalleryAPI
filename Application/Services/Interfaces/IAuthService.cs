namespace Application.Services.Interfaces;

using System.Security.Claims;
using Application.Models;

public interface IAuthService
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<bool> RegisterUserAsync(User user, string password);
    Task<bool> ChangePasswordAsync(User user, string newPassword);
    Task<string> GetNewJsonWebTokenAsync(User user,
                                         string issuer,
                                         string audience,
                                         DateTime expiration,
                                         string secretKey,
                                         params Claim[] claims);
}