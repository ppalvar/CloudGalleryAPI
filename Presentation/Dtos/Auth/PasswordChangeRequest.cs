namespace Presentation.Dtos.Auth;

public class PasswordChangeRequest
{
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}