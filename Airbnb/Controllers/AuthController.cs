using Microsoft.AspNetCore.Mvc;
using Airbnb.Application.DTOs;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;

namespace Airbnb.Controllers;

[ApiController]
[Route("api/v1/controllers")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
    {
        _authService = authService;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterUserAsync(dto);
        
        if (result.Succeeded)
            return Ok("User registered successfully!");
        
        return BadRequest(result.Errors);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.LoginAsync(dto.Email, dto.Password);

        if (token == null)
        {
            return Unauthorized("Invalid username or password");
        }

        return Ok(new { Token = token });
    }
}