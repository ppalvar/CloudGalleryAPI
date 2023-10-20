namespace Contracts.Dtos.Gallery;

public class PhotoDetailDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? PhotoFormat { get; set; } = null!;
    public int PhotoHeight { get; set; }
    public int PhotoWidth { get; set; }
}