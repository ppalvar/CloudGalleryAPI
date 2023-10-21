namespace Infraestructure.ExternalServices.Mocks;

using System.Threading.Tasks;
using Application.ExternalServices.Interfaces;

public class ThumbnailGeneratorServiceMock : IThumbnailGenerationService
{
    public Task<byte[]> GetThumbnail(byte[] image)
    {
        return Task.FromResult(image);
    }
}