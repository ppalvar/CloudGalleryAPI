namespace Infraestructure.ExternalServices.Local;

using Application.ExternalServices.Interfaces;

public class PhotoStorageServiceLocal : IPhotoStorageService
{
    private const string basePath = "./";

    public Task<bool> DeleteFile(string fileName)
    {
        System.IO.File.Delete(Path.Combine(basePath, "cloud-gallery-photos", fileName));
        return Task.FromResult(true);
    }

    public async Task<byte[]> RetrieveFile(string fileName)
    {
        string path = Path.Combine(basePath, "cloud-gallery-photos", fileName);

        return await System.IO.File.ReadAllBytesAsync(path);
    }

    public async Task<bool> SaveFile(byte[] file, string fileName)
    {
        string path = Path.Combine(basePath, "cloud-gallery-photos", fileName);

        await System.IO.File.WriteAllBytesAsync(path, file);
        return true;
    }
}