namespace Presentation.Controllers;

using Contracts.Dtos.Common;
using Contracts.Dtos.Gallery;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("photos")]
public class PhotoController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PhotoInfoDto>> GetPhotos()
    {
        return Ok();
    }


    [HttpGet("{id}/thumbnail")]
    public ActionResult<FileStreamResult> GetThumbnail(int id)
    {
        return Ok();
    }

    [HttpGet("{id}/full")]
    public ActionResult<FileStreamResult> GetFullPhoto(int id)
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public ActionResult<PhotoDetailDto> GetDetails(int id)
    {
        return Ok();
    }

    [HttpPost]
    public ActionResult<StatusResponse> PostPhoto([FromForm] IFormFile file, [FromBody] PhotoUploadDto request)
    {
        return Ok(file.Length);
    }

    [HttpDelete("{id}")]
    public ActionResult<StatusResponse> DeletePhoto(int id)
    {
        return Ok();
    }
}