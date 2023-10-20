namespace Presentation.Controllers;

using Contracts.Dtos.Auth;
using Contracts.Dtos.Auth.Common;
using Contracts.Dtos.Common;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<LoginResponse> Login(UserLoginRequest request)
    {
        return Ok();
    }


    [HttpPost("register")]
    public ActionResult<StatusResponse> Register(UserRegisterRequest request)
    {
        return Ok();
    }


    [HttpPost("password-change")]
    public ActionResult<StatusResponse> PasswordChange(PasswordChangeRequest request)
    {
        return Ok();
    }


    [HttpPost("password-reset")]
    public ActionResult<StatusResponse> PasswordReset(PasswordResetRequest request)
    {
        return Ok();
    }
}