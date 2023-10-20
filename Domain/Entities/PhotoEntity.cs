namespace Domain.Entities;

public class PhotoEntity
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
}