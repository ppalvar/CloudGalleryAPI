namespace Contracts.Dtos.Auth;

public class PasswordResetRequest
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}