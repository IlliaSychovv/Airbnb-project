using System.Text.Json;
using Shared.Kafka.Interfaces;
using AuditService.Application.DTO;
using AuditService.Domain.Entity;
using AuditService.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace AuditService.Infrastructure.Services;

public class ProfileKafkaHandler : IKafkaMessageHandler<AuditDto>
{
    private readonly MongoDbContext _mongoContext;
    private readonly ILogger<ProfileKafkaHandler> _logger;

    public ProfileKafkaHandler(MongoDbContext mongoContext, ILogger<ProfileKafkaHandler> logger)
    {
        _mongoContext = mongoContext;
        _logger = logger;
    }

    public async Task HandleMessage(string message, string key, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kafka message received: {Message}", message);
        
        var json = JsonDocument.Parse(message);
        var userId = Guid.Parse(json.RootElement.GetProperty("Id").GetString()!); 
        
        var entity = new Audit
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            ProfileJson = message
        };

        await _mongoContext.Audits.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }
}