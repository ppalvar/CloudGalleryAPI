namespace Presentation.Controllers;

using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Contracts.Dtos.Auth;
using Contracts.Dtos.Auth.Common;
using Contracts.Dtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController, Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IMapper mapper;
    private readonly ConfigurationManager _config;

    public AuthController(IAuthService authService, IMapper mapper, ConfigurationManager config)
    {
        this.authService = authService;
        this.mapper = mapper;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(UserLoginRequest request)
    {
        var user = await authService.GetUserByUsernameAsync(request.UserName);
        if (user is null) return NotFound(StatusResponse.Error("User does not exists."));

        bool isPasswordCorrect = await authService.CheckPasswordAsync(user, request.Password);
        if (!isPasswordCorrect) return BadRequest(StatusResponse.Error("Wrong password."));

        var expiration = DateTime.Now.AddMinutes(30);
        var token = await authService.GetNewJsonWebTokenAsync(user: user,
                                                              issuer: _config["Jwt:Issuer"],
                                                              audience: _config["Jwt:Audience"],
                                                              expiration: expiration,
                                                              secretKey: _config["Jwt:Secret"]);

        return Ok(new LoginResponse(token, expiration));
    }


    [HttpPost("register")]
    public async Task<ActionResult<StatusResponse>> Register(UserRegisterRequest request)
    {
        var user = mapper.Map<UserRegisterRequest, User>(request);

        bool success = await authService.RegisterUserAsync(user, request.Password);

        if (!success)
        {
            return BadRequest(StatusResponse.Error("Couldn't create user."));
        }
        return Ok(StatusResponse.Success("User registered successfully."));
    }

    [Authorize]
    [HttpPost("password-change")]
    public async Task<ActionResult<StatusResponse>> PasswordChange(PasswordChangeRequest request)
    {
        var _user = this.User.Identity;
        if (_user is null || _user.Name is null) return Forbid();

        var user = await authService.GetUserByUsernameAsync(_user.Name);
        if (user is null) return NotFound(StatusResponse.Error("User Not Found"));

        if (!await authService.CheckPasswordAsync(user, request.OldPassword)) return Forbid();

        await authService.ChangePasswordAsync(user, request.NewPassword);


        return Ok(StatusResponse.Success("Password changed successfully"));
    }
}