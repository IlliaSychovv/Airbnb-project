using Airbnb.DTOs;
using Microsoft.AspNetCore.Mvc;
using Airbnb.DTOs.Interfaces;
 
namespace Airbnb.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterUserAsync(dto);
        
        if (result.Succeeded)
            return Ok("User registered successfully!");
        
        return BadRequest(result.Errors);
    }
}