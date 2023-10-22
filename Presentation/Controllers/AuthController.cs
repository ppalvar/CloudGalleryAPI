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

        if (user is null)
        {
            return NotFound(
                new StatusResponse
                {
                    Status = "Error",
                    Message = "User does not exists"
                }
            );
        }

        bool isPasswordCorrect = await authService.CheckPasswordAsync(user, request.Password);

        if (!isPasswordCorrect)
        {
            return BadRequest(
                new StatusResponse
                {
                    Status = "Error",
                    Message = "Wrong password"
                }
            );
        }

        var expiration = DateTime.Now.AddMinutes(30);

        var token = await authService.GetNewJsonWebTokenAsync(user: user,
                                                              issuer: _config["Jwt:Issuer"],
                                                              audience: _config["Jwt:Audience"],
                                                              expiration: expiration,
                                                              secretKey: _config["Jwt:Secret"]);

        return Ok(
            new LoginResponse
            {
                Token = token,
                TokenExpiration = expiration
            }
        );
    }


    [HttpPost("register")]
    public async Task<ActionResult<StatusResponse>> Register(UserRegisterRequest request)
    {
        var user = mapper.Map<UserRegisterRequest, User>(request);

        bool success = await authService.RegisterUserAsync(user, request.Password);

        if (!success)
        {
            return BadRequest(
                new StatusResponse
                {
                    Status = "Error",
                    Message = "Couldn't create user"
                }
            );
        }
        return Ok(
            new StatusResponse
            {
                Status = "Success",
                Message = "User registered successfully."
            }
        );
    }

    [Authorize]
    [HttpPost("password-change")]
    public async Task<ActionResult> PasswordChange(PasswordChangeRequest request)
    {
        var _user = this.User.Identity;

        if (_user is null || _user.Name is null) return Forbid();

        var user = await authService.GetUserByUsernameAsync(_user.Name);

        if (user is null) return NotFound();

        if (!await authService.CheckPasswordAsync(user, request.OldPassword)) return Forbid();

        await authService.ChangePasswordAsync(user, request.NewPassword);

        return Ok();
    }
}