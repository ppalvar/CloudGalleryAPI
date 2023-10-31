namespace Application.Pipelines.Interfaces;

public interface IPhotoProcessingPipeline {
    Task<bool> ProccessAndUpload(byte[] photo, string fileName);
    Task<byte[]> RetrieveImage(string fileName, bool isThumbnail = false);
    Task<bool> DeleteImage(string filename);
}