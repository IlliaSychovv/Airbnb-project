using System.Text.Json;
using AuditService.Application.DTO;
using AuditService.Domain.Entity;
using AuditService.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Shared.Kafka.Interfaces;

namespace AuditService.Infrastructure.Services;

public class ApartmentKafkaHandler : IKafkaMessageHandler<AuditApartmentDto>
{
    private readonly MongoDbContext _mongoContext;
    private readonly ILogger<ApartmentKafkaHandler> _logger;

    public ApartmentKafkaHandler(MongoDbContext mongoContext, ILogger<ApartmentKafkaHandler> logger)
    {
        _mongoContext = mongoContext;
        _logger = logger;
    }

    public async Task HandleMessage(string message, string key, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Kafka message received: {Message}", message);
        
        var json = JsonDocument.Parse(message);
        var apartmentId = Guid.Parse(json.RootElement.GetProperty("Id").GetString()!);
        
        var entity = new Apartment
        {
            Id = Guid.NewGuid(),
            ApartmentId = apartmentId,
            ProfileJson = message,
            CreatedAt = DateTime.UtcNow
        };
        
        await _mongoContext.Apartments.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }
}