namespace Presentation.Controllers;

using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Contracts.Dtos.Common;
using Contracts.Dtos.Gallery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("photos")]
public class PhotoController : ControllerBase
{
    private readonly IPhotoService photoService;
    private readonly IAuthService authService;
    private readonly IMapper mapper;

    public PhotoController(IPhotoService photoService, IMapper mapper, IAuthService authService)
    {
        this.photoService = photoService;
        this.mapper = mapper;
        this.authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PhotoInfoDto>>> GetPhotos()
    {
        IEnumerable<Photo> photos = await photoService.GetPhotosAsync();

        return Ok(
            mapper.Map<IEnumerable<Photo>, IEnumerable<PhotoInfoDto>>(photos)
        );
    }


    [HttpGet("{id}/thumbnail")]
    public async Task<IActionResult> GetThumbnail(int id)
    {
        try
        {
            var thumbnail = await photoService.GetPhotoFileByIdAsync(id, true);
            var photoInfo = await photoService.GetPhotoByIdAsync(id);

            return File(thumbnail, photoInfo!.Format);
        }
        catch (Exception e)
        {
            return BadRequest(StatusResponse.Error(e.Message));
        }
    }

    [HttpGet("{id}/full")]
    public async Task<IActionResult> GetFullPhoto(int id)
    {
        try
        {
            var photoFile = await photoService.GetPhotoFileByIdAsync(id);
            var photoInfo = await photoService.GetPhotoByIdAsync(id);

            return File(photoFile, photoInfo!.Format);
        }
        catch (Exception e)
        {
            return BadRequest(StatusResponse.Error(e.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PhotoDetailDto>> GetDetails(int id)
    {
        var photoInfo = await photoService.GetPhotoByIdAsync(id);

        if (photoInfo is null)
        {
            return NotFound(StatusResponse.Error($"Photo with id {id} was not found."));
        }

        return mapper.Map<Photo, PhotoDetailDto>(photoInfo);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<StatusResponse>> PostPhoto([FromForm] PhotoUploadRequest request)
    {
        if (request.File.Length == 0)
        {
            return BadRequest(StatusResponse.Error("Not File Attached."));
        }

        var photo = mapper.Map<PhotoUploadRequest, Photo>(request);
        var stream = new MemoryStream();

        await request.File.CopyToAsync(stream);

        photo.Format = request.File.ContentType;

        var _user = this.User.Identity;

        //this line checks if the name exists, then stores in 'user' the 
        //User object to use it as owner of the uploaded photo
        if (_user is null || _user.Name is null || await authService.GetUserByUsernameAsync(_user.Name) is not User user)
        {
            return BadRequest(StatusResponse.Error("Not user executing this operation."));
        }

        photo.Owner = user;

        await photoService.UploadPhotoAsync(photo, stream.ToArray());

        return Ok(StatusResponse.Success("Created successfully."));
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<StatusResponse>> DeletePhoto(int id)
    {
        var _user = this.User.Identity;

        //this line checks if the name exists, then stores in 'user' the 
        //User object to use it as owner of the uploaded photo
        if (_user is null || _user.Name is null || await authService.GetUserByUsernameAsync(_user.Name) is not User user)
        {
            return BadRequest(StatusResponse.Error("Not user executing this operation."));
        }

        var photo = await photoService.GetPhotoByIdAsync(id);

        if (photo != null && photo.Owner == user)
        {
            bool success = await photoService.DeletePhotoByIdAsync(id);
            if (success) return Ok(StatusResponse.Success("The photo was deleted successfully."));
        }
        else
        {
            return BadRequest(StatusResponse.Error($"The image does not belong to {user.Username} so they cannot delete it."));
        }


        return NotFound(
            StatusResponse.Error($"The image with id {id} was not found."));
    }
}