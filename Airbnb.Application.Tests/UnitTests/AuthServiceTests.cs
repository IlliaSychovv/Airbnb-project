using Airbnb.Application.CreatedEvent;
using Airbnb.Application.DTO.Authorization;
using Airbnb.Application.Interfaces;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.Services;
using Airbnb.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using Moq;

namespace Airbnb.Application.Tests.UnitTests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterUserAsync_ShouldReturnSuccessResult_WhenUserIsCreated()
    {
        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "password"
        };

        var userManagerMock = new Mock<IUserManagerWrapper>();
        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);
        
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        
        var eventSenderMock = new Mock<IEventSender>();

        var authService = new AuthService(userManagerMock.Object, jwtTokenServiceMock.Object, eventSenderMock.Object);
        
        var result = await authService.RegisterUserAsync(dto);
        
        result.Succeeded.ShouldBeTrue(); 
        eventSenderMock.Verify(m => 
                m.SendEvent(
                    It.IsAny<string>(), 
                    It.IsAny<UserCreatedEvent>()),
            Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_ShouldReturn_FailedResult_WhenUserIsNotCreated()
    {
        var dto = new RegisterDto
        {
            Email = "test@test.com",
            Password = "password",
        };
        
        var failedResult = IdentityResult.Failed( new IdentityError
        {
            Description = "Unfortunately user is not created"
        });

        var userManagerMock = new Mock<IUserManagerWrapper>();
        userManagerMock
            .Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(failedResult);
        
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        
        var eventSenderMock = new Mock<IEventSender>();
        
        var authService = new AuthService(userManagerMock.Object, jwtTokenServiceMock.Object, eventSenderMock.Object);
        
        var result = await authService.RegisterUserAsync(dto);
        
        result.Succeeded.ShouldBeFalse(); 
        eventSenderMock.Verify(m => 
                m.SendEvent(
                    It.IsAny<string>(), 
                    It.IsAny<UserCreatedEvent>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenLoginIsSuccessful()
    {
        var testRoles = new List<string> { "User" };
        var expectedToken = "mocked-token";
        var testUser = new ApplicationUser
        {
            Email = "test@test.com",
        };
        
        var userManagerMock = new Mock<IUserManagerWrapper>();
        userManagerMock
            .Setup(m => m.FindByNameAsync("test@test.com"))
            .ReturnsAsync(testUser);

        userManagerMock
            .Setup(m => m.CheckPasswordAsync(testUser, "password"))
            .ReturnsAsync(true);
        
        userManagerMock
            .Setup(m => m.GetRolesAsync(testUser))
            .ReturnsAsync(testRoles);
        
        var jwtTokenServiceMock = new Mock<IJwtTokenService>();
        jwtTokenServiceMock
            .Setup(m => m.GenerateToken(testUser, testRoles))
            .Returns(expectedToken);
        
        var eventSenderMock = new Mock<IEventSender>();
        
        var authService = new AuthService(userManagerMock.Object, jwtTokenServiceMock.Object, eventSenderMock.Object);
        
        var result = await authService.LoginAsync("test@test.com", "password");
        
        result.ShouldBe(expectedToken);
        eventSenderMock.Verify(m => 
                m.SendEvent(
                    It.IsAny<string>(), 
                    It.IsAny<UserCreatedEvent>()),
            Times.Never);
    }
}