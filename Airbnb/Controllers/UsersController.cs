using Airbnb.Application.DTO.Authorization;
using Airbnb.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UsersController : ControllerBase
{
    private readonly IAuthService _authService;

    public UsersController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateDto dto, string userId)
    {
        await _authService.UpdateUser(dto, userId);
        return NoContent();
    }
}