namespace Presentation.Controllers;

using Application.Models;
using Application.Services.Interfaces;
using AutoMapper;
using Presentation.Dtos.Auth;
using Presentation.Dtos.Auth.Common;
using Presentation.Dtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Validators;

[ApiController, Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    private readonly IMapper mapper;
    private readonly ConfigurationManager _config;
    private readonly UserRegisterValidator validationRules;

    public AuthController(IAuthService authService, IMapper mapper, ConfigurationManager config, UserRegisterValidator validationRules)
    {
        this.authService = authService;
        this.mapper = mapper;
        _config = config;
        this.validationRules = validationRules;
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
        //only the first error encountered will be displayed to the user
        var result = await validationRules.ValidateAsync(request);
        if (!result.IsValid) return BadRequest(StatusResponse.Error(result.Errors[0].ErrorMessage));

        var user = mapper.Map<UserRegisterRequest, User>(request);

        bool success = await authService.RegisterUserAsync(user, request.Password);
        if (!success) return BadRequest(StatusResponse.Error("Couldn't create user."));

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