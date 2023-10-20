namespace Application.ExternalServices.Interfaces;

public interface IPhotoStorageService
{
    Task<bool> SaveFile(byte[] file, string fileName);
    Task<byte[]> RetrieveFile(string fileName);
    Task<bool> DeleteFile(string fileName);
}