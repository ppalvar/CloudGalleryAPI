namespace Contracts.Dtos.Auth;

public class UserLoginRequest
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}