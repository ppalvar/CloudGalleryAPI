namespace Application.Services.Interfaces;

using Application.Models;
using Domain.Entities;

public interface IPhotoService
{
    Task<IEnumerable<Photo>> GetPhotosAsync();
    Task<Photo?> GetPhotoByIdAsync(long Id);
    Task<bool> UploadPhotoAsync(Photo photoInfo, byte[] photoFile);
    Task<bool> DeletePhotoByIdAsync(long Id);
    Task<byte[]> GetPhotoThumbnailByIdAsync(long Id);
    Task<byte[]> GetPhotoFileByIdAsync(long Id, bool isThumbnail = false);
}