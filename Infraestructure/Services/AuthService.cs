namespace Infraestructure.Services;

using System.Security.Claims;
using System.Security.Cryptography;
using Application.Models;
using Application.Services.Interfaces;
using Infraestructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

public class AuthService : IAuthService
{
    private readonly CloudGalleryDbContext dbContext;

    public AuthService(CloudGalleryDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<bool> RegisterUserAsync(User user, string password)
    {
        if (await UserExists(user))
        {
            return false;
        }

        (user.PasswordSalt, user.PasswordHash) = ComputePasswordHashAndSalt(password);

        try
        {
            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }
        catch { return false; }

        return true;
    }

    public async Task<bool> ChangePasswordAsync(User user, string newPassword)
    {
        if (!await UserExists(user))
        {
            return false;
        }

        (user.PasswordSalt, user.PasswordHash) = ComputePasswordHashAndSalt(newPassword);

        dbContext.Entry(user).State = EntityState.Modified;

        try { await dbContext.SaveChangesAsync(); }
        catch { return false; }

        return true;
    }

    public async Task<string> GetNewJsonWebTokenAsync(User user,
                                                      string issuer,
                                                      string audience,
                                                      DateTime expiration,
                                                      string secretKey,
                                                      params Claim[] claims)
    {
        if (!await UserExists(user)) return string.Empty;

        var authKey = new SymmetricSecurityKey(GetBytes(secretKey));
        var credentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256);
        var _claims = claims.ToList();

        _claims.Add(new Claim(ClaimTypes.Name, user.Username));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: _claims,
            expires: expiration,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await dbContext.Users.Where(usr => usr.Username == username).FirstOrDefaultAsync();
    }

    public Task<bool> CheckPasswordAsync(User user, string password)
    {
        byte[] hash, salt;

        (salt, hash) = ComputePasswordHashAndSalt(password, user.PasswordSalt);
        bool check = hash.SequenceEqual(user.PasswordHash);

        return Task.FromResult(check);
    }

    private byte[] GetBytes(string str)
    {
        return System.Text.Encoding.UTF8.GetBytes(str);
    }

    private (byte[], byte[]) ComputePasswordHashAndSalt(string password, byte[]? key = null)
    {
        using (var hmac = (key is null) ? new HMACSHA256() : new HMACSHA256(key))
        {
            byte[] passwordSalt = hmac.Key;
            byte[] passwordHash = hmac.ComputeHash(GetBytes(password));

            return (passwordSalt, passwordHash);
        }
    }

    private async Task<bool> UserExists(User user)
    {
        var userExists = await GetUserByUsernameAsync(user.Username);

        return userExists is not null;
    }
}