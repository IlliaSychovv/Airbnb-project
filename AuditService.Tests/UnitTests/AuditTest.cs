using AuditService.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AuditService.Tests.UnitTests;

public class AuditTest
{
    private readonly Mock<IAuditRepository> _auditRepositoryMock;
    private readonly Mock<IMonolithClient> _monolithClientMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    
}