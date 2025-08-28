using System.Text.Json;
using Shared.Kafka.Interfaces;
using AuditService.Application.DTOs;
using AuditService.Domain.Entity;
using AuditService.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace AuditService.Infrastructure.Services;

public class ProfileKafkaHandler : IKafkaMessageHandler<AuditDto>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ProfileKafkaHandler> _logger;

    public ProfileKafkaHandler(AppDbContext dbContext, ILogger<ProfileKafkaHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task HandleMessage(string message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kafka message received: {Message}", message);
        
        var json = JsonDocument.Parse(message);
        var userId = Guid.Parse(json.RootElement.GetProperty("Id").GetString()!); 
        
        var entity = new Audit
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ProfileJson = message
        };

        _dbContext.Audits.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}