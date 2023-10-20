using Application.ExternalServices.Interfaces;

namespace Application.Pipelines;

public class PhotoProcessingPipeline
{
    private readonly IVirusScannerService virusScannerService;
    private readonly IThumbnailGenerationService thumbnailGenerationService;
    private readonly IPhotoStorageService photoStorageService;
    private readonly IFileEncryptService fileEncryptService;


    public PhotoProcessingPipeline(IVirusScannerService virusScannerService, IThumbnailGenerationService thumbnailGenerationService, IPhotoStorageService photoStorageService, IFileEncryptService fileEncryptService)
    {
        this.virusScannerService = virusScannerService;
        this.thumbnailGenerationService = thumbnailGenerationService;
        this.photoStorageService = photoStorageService;
        this.fileEncryptService = fileEncryptService;
    }

    public async Task<bool> ProccessAndUpload(byte[] photo, string fileName)
    {
        if (!await virusScannerService.IsFileSecure(photo)) return false;

        byte[] thumbnail = await thumbnailGenerationService.GetThumbnail(photo);

        thumbnail = await fileEncryptService.GetEncryptedFile(thumbnail);
        photo = await fileEncryptService.GetEncryptedFile(photo);

        await photoStorageService.SaveFile(photo, fileName);
        await photoStorageService.SaveFile(thumbnail, fileName + ".thumbnail");

        return true;
    }

    public async Task<byte[]> RetrieveImage(string fileName, bool isThumbnail = false)
    {
        if (isThumbnail)
        {
            fileName = $"{fileName}.thumbnail";
        }

        byte[] file = await photoStorageService.RetrieveFile(fileName);
        file = await fileEncryptService.GetDecryptedFile(file);

        return file;
    }

    public async Task<bool> DeleteImage(string filename)
    {
        return await photoStorageService.DeleteFile(filename);
    }
}