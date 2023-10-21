namespace Infraestructure.ExternalServices.Mocks;

using System.Threading.Tasks;
using Application.ExternalServices.Interfaces;

public class VirusScannerServiceMock : IVirusScannerService
{
    public Task<bool> IsFileSecure(byte[] file)
    {
        return Task.FromResult(true);
    }
}