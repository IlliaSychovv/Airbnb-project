using Airbnb.Application.DTOs;
using Airbnb.Application.Interfaces;
using Airbnb.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Airbnb.Application.Tests;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _controller = new AuthController(_authServiceMock.Object, _jwtTokenServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnOkResult()
    {
        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "password"
        };

        _authServiceMock
            .Setup(m => m.RegisterUserAsync(dto))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Register(dto);
        
        Assert.IsType<OkObjectResult>(result);
    }
    
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsOkObjectResult()
    {
        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "password"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto.Email, loginDto.Password))
            .ReturnsAsync("mocked-token");

        var result = await _controller.Login(loginDto);
        
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void Logout_ReturnsOkResultWithMessage()
    {
        var result = _controller.Logout();
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("You are a logged out person!", okResult.Value);
    }
}