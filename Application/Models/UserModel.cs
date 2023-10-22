namespace Application.Models;

using Domain.Entities;

public class User : UserEntity
{
    public bool IsEmailVerified { get; set; } = false;
    public bool IsBanned { get; set; } = false;
}