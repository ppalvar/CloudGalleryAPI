namespace Presentation.Controllers;

using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Contracts.Dtos.Common;
using Contracts.Dtos.Gallery;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("photos")]
public class PhotoController : ControllerBase
{
    private readonly IPhotoService service;
    private readonly IMapper mapper;

    public PhotoController(IPhotoService service, IMapper mapper)
    {
        this.service = service;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhotoInfoDto>>> GetPhotos()
    {
        IEnumerable<Photo> photos = await service.GetPhotosAsync();

        return Ok(
            mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoInfoDto>>(photos)
        );
    }


    [HttpGet("{id}/thumbnail")]
    public async Task<IActionResult> GetThumbnail(int id)
    {
        try
        {
            var thumbnail = await service.GetPhotoFileByIdAsync(id, true);
            var photoInfo = await service.GetPhotoByIdAsync(id);

            return File(thumbnail, photoInfo!.Format);
        }
        catch (Exception e)
        {
            return BadRequest(
                new StatusResponse
                {
                    Status = "Error",
                    Message = e.Message
                }
            );
        }
    }

    [HttpGet("{id}/full")]
    public async Task<IActionResult> GetFullPhoto(int id)
    {
        try
        {
            var photoFile = await service.GetPhotoFileByIdAsync(id);
            var photoInfo = await service.GetPhotoByIdAsync(id);

            return File(photoFile, photoInfo!.Format);
        }
        catch (Exception e)
        {
            return BadRequest(
                new StatusResponse
                {
                    Status = "Error",
                    Message = e.Message
                }
            );
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PhotoDetailDto>> GetDetails(int id)
    {
        var photoInfo = await service.GetPhotoByIdAsync(id);

        if (photoInfo is null)
        {
            return NotFound(
                new StatusResponse
                {
                    Status = "Error",
                    Message = $"Photo with id {id} was not found."
                }
            );
        }

        return mapper.Map<Photo, PhotoDetailDto>(photoInfo);
    }

    [HttpPost]
    public async Task<ActionResult<StatusResponse>> PostPhoto([FromForm] PhotoUploadRequest request)
    {
        if (request.File.Length == 0)
        {
            return Ok(new StatusResponse
            {
                Status = "Error",
                Message = "Not File Attached"
            });
        }

        var photo = mapper.Map<PhotoUploadRequest, Photo>(request);
        var stream = new MemoryStream();

        await request.File.CopyToAsync(stream);

        photo.Format = request.File.ContentType;


        await service.UploadPhotoAsync(photo, stream.ToArray());

        return Ok(new StatusResponse
        {
            Status = "Success",
            Message = "Created successfully"
        });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<StatusResponse>> DeletePhoto(int id)
    {
        bool success = await service.DeletePhotoByIdAsync(id);

        if (success)
        {
            return Ok(
                new StatusResponse
                {
                    Status = "Success",
                    Message = "The photo was deleted successfully."
                }
            );
        }

        return NotFound(
            new StatusResponse
            {
                Status = "Error",
                Message = $"The image with id {id} was not found."
            }
        );
    }
}