namespace Contracts.Dtos.Auth.Common;

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public DateTime TokenExpiration { get; set; }

    public LoginResponse(string token, DateTime tokenExpiration)
    {
        Token = token;
        TokenExpiration = tokenExpiration;
    }
}