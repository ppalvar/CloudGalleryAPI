namespace Application.ExternalServices.Interfaces;

public interface IVirusScannerService
{
    Task<bool> IsFileSecure(byte[] file);
}