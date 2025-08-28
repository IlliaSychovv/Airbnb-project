using Airbnb.Application.DTO.Authorization;
using Microsoft.AspNetCore.Mvc;
using Airbnb.Application.Interfaces.Services;

namespace Airbnb.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterUserAsync(dto);

        if (result.Succeeded)
            return Created(string.Empty, new { Email = dto.Email });
        
        return BadRequest(result.Errors);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto.Email, dto.Password);

        if (token == null)
            return Unauthorized("Invalid username or password");

        return Ok(new { Token = token });
    }
}