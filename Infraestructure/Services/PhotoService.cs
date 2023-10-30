namespace Infraestructure.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models;
using Application.Pipelines;
using Application.Services.Interfaces;
using Infraestructure.DbContexts;
using Microsoft.EntityFrameworkCore;

public class PhotoService : IPhotoService
{
    private readonly PhotoProcessingPipeline photoProcessingPipeline;
    private readonly CloudGalleryDbContext dbContext;

    public PhotoService(PhotoProcessingPipeline photoProcessingPipeline, CloudGalleryDbContext dbContext)
    {
        this.photoProcessingPipeline = photoProcessingPipeline;
        this.dbContext = dbContext;
    }

    public async Task<bool> DeletePhotoByIdAsync(long Id)
    {
        var photo = await dbContext.Photos.FindAsync(Id);

        if (photo is null)
        {
            return false;
        }

        dbContext.Photos.Remove(photo);
        await dbContext.SaveChangesAsync();

        return await photoProcessingPipeline.DeleteImage(Id.ToString())
            && await photoProcessingPipeline.DeleteImage(Id.ToString() + ".thumbnail");
    }

    public async Task<Photo?> GetPhotoByIdAsync(long Id)
    {
        return await dbContext.Photos
                     .Include(photo => photo.Owner)
                     .Where(photo => photo.Id == Id)
                     .FirstOrDefaultAsync();

    }

    public async Task<byte[]> GetPhotoFileByIdAsync(long Id, bool isThumbnail = false)
    {
        var photo = await GetPhotoByIdAsync(Id);

        if (photo is null)
        {
            throw new FileNotFoundException($"The photo file with id {Id} does not exists.");
        }

        try
        {
            return await photoProcessingPipeline.RetrieveImage(Id.ToString(), isThumbnail);
        }
        catch
        {
            throw new FileNotFoundException(@$"The file with id {Id} exists in the database but not in the
                                              filesystem. This may be an internal error please contact support");
        }
    }

    public async Task<IEnumerable<Photo>> GetPhotosAsync()
    {
        return await dbContext.Photos.Include(photo => photo.Owner).ToListAsync();
    }

    public async Task<byte[]> GetPhotoThumbnailByIdAsync(long Id)
    {
        return await GetPhotoFileByIdAsync(Id, true);
    }

    public async Task<bool> UploadPhotoAsync(Photo photoInfo, byte[] photoFile)
    {
        var last = dbContext.Photos.OrderBy(pht => pht.Id).LastOrDefault();
        long Id = 1L;
        bool success = false;

        if (last is not null)
        {
            Id = last.Id + 1;
        }

            success = await photoProcessingPipeline.ProccessAndUpload(photoFile, Id.ToString());
        try
        {
        }
        catch
        {
            throw new FileLoadException($"Error while saving file with id {Id}");
        }

        if (success)
        {
            photoInfo.Id = Id;
            await dbContext.AddAsync(photoInfo);
            await dbContext.SaveChangesAsync();
        }

        return success;
    }
}