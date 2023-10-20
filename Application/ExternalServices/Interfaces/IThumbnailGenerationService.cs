namespace Application.ExternalServices.Interfaces;

public interface IThumbnailGenerationService
{
    Task<byte[]> GetThumbnail(byte[] image);
}