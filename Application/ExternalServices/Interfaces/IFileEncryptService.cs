namespace Application.ExternalServices.Interfaces;

public interface IFileEncryptService
{
    Task<byte[]> GetEncryptedFile(byte[] file, byte[]? key=null);
    Task<byte[]> GetDecryptedFile(byte[] file, byte[]? key=null);
}