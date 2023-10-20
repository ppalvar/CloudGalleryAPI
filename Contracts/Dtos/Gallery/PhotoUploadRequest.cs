using Microsoft.AspNetCore.Http;

namespace Contracts.Dtos.Gallery;

public class PhotoUploadRequest
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IFormFile File { get; set; } = null!;
}