namespace Domain.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public byte[] PasswordHash { get; set; } = null!;
    public byte[] PasswordSalt { get; set; } = null!;

    public IEnumerable<PhotoEntity> Photos { get; set; } = null!;
}