namespace Application.Models;

using Domain.Entities;

public class Photo : PhotoEntity
{
    public string Format { get; set; } = null!;
    public int Heigth { get; set; }
    public int Width { get; set; }
}