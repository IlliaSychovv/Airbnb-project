using Airbnb.Application.DTO.Authorization;
using Airbnb.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateDto dto, string userId)
    {
        await _userService.UpdateUserAsync(dto, userId);
        return NoContent();
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile(Guid userId)
    {
        var userProfile = await _userService.GetUserProfileAsync(userId);
        return Ok(userProfile);
    }
}