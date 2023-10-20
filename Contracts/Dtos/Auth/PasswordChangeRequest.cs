namespace Contracts.Dtos.Auth;

public class PasswordChangeRequest
{
    public string UserName { get; set; } = null!;
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}