namespace Infraestructure.ExternalServices.Mocks;

using System.Threading.Tasks;
using Application.ExternalServices.Interfaces;

public class FileEncryptServiceMock : IFileEncryptService
{
    public Task<byte[]> GetDecryptedFile(byte[] file, byte[]? key = null)
    {
        return Task.FromResult(file);
    }

    public Task<byte[]> GetEncryptedFile(byte[] file, byte[]? key = null)
    {
        return Task.FromResult(file);
    }
}