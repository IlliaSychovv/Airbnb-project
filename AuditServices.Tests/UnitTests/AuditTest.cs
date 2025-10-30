using AuditService.Application.DTO;
using AuditService.Application.Interfaces;
using AuditService.Domain.Entity;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;

namespace AuditService.Tests.UnitTests;

public class AuditTest
{
    private readonly Mock<IAuditRepository> _auditRepositoryMock;
    private readonly Mock<IMonolithClient> _monolithClientMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly IAuditService _auditService;

    public AuditTest()
    {
        _auditRepositoryMock = new Mock<IAuditRepository>();
        _monolithClientMock = new Mock<IMonolithClient>();
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _auditService = new Application.Services.AuditService(_auditRepositoryMock.Object, _monolithClientMock.Object, _httpContextAccessorMock.Object);
    }

    [Fact]
    public async Task GetUserChangesAsync_ShouldReturnUserChanges_WhenWeCallMethod()
    {
        var userId = Guid.NewGuid();
        var port = 1234;

        var auditChanges = new List<Audit>
        {
            new Audit
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId.ToString()),
                ProfileJson = "some audit json",
                CreatedAt = DateTime.Now
            }
        };
        
        var userLogins = new UserLoginsDto
        {
            Name = "Name",
            Email = "Email",
            PhoneNumber = "PhoneNumber",
            CreatedAt = DateTime.UtcNow
        };
        
        _auditRepositoryMock.Setup(x => x.GetUserChanges(userId))
            .ReturnsAsync(auditChanges);

        _monolithClientMock.Setup(x => x.GetUserLoginAsync(userId))
            .ReturnsAsync(userLogins);
        
        var context = new DefaultHttpContext();
        context.Connection.LocalPort = port;
        
        _httpContextAccessorMock.Setup(x => x.HttpContext)
            .Returns(context);
        
        var result = await _auditService.GetUserChangesAsync(userId);
        
        result.ShouldNotBeNull();
        result.Port.ShouldBe(port);
        result.User.ShouldBe(userLogins);
        result.Changes.Count.ShouldBe(auditChanges.Count);
        
        _auditRepositoryMock.Verify(x => x.GetUserChanges(userId), Times.Once);
        _monolithClientMock.Verify(x => x.GetUserLoginAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetApartmentsChangesAsync_ShouldReturnApartmentChanges_WhenWeCallMethod()
    {
        var apartmentId = Guid.NewGuid();
        var apartmentChanges = new List<Apartment>
        {
            new Apartment
            {
                Id = apartmentId,
                ApartmentId = Guid.Parse(apartmentId.ToString()),
                ProfileJson = "some audit json",
                CreatedAt = DateTime.Now
            }
        };

        _auditRepositoryMock.Setup(x => x.GetApartmentChanges(apartmentId))
            .ReturnsAsync(apartmentChanges);
        
        var result = await _auditService.GetApartmentsAsync(apartmentId);
        
        result.ShouldNotBeNull();
        result.Count.ShouldBe(apartmentChanges.Count);
        
        _auditRepositoryMock.Verify(x => x.GetApartmentChanges(apartmentId), Times.Once);
    }
}