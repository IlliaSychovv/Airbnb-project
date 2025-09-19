using Airbnb.Application.DTO.Authorization;
using Airbnb.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UsersController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateDto dto, string userId)
    {
        await _authService.UpdateUser(dto, userId);
        return NoContent();
    }

    [HttpGet("logins")]
    public async Task<IActionResult> GetLogins(Guid userId)
    {
        var userLogin = await _userService.GetUserLoginsAsync(userId);
        return Ok(userLogin);
    }
}