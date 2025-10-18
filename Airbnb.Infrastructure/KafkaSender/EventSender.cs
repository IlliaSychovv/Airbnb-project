using System.Text.Json;
using Airbnb.Application.Interfaces;
using Airbnb.Domain.Entities;
using Airbnb.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Shared.Kafka.Interfaces;

namespace Airbnb.Infrastructure.KafkaSender;

public class EventSender : IEventSender
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IKafkaProducer _producer;

    public EventSender(IKafkaProducer producer, IServiceScopeFactory serviceScopeFactory)
    {
        _producer = producer;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task SendEvent<TEvent>(object key, TEvent message)
    {
        var topic = KafkaTopicRegistry.GetTopicFor<TEvent>();
        
        var jsonMessage = JsonSerializer.Serialize(message);
        
        await _producer.ProduceAsync(topic, key.ToString(), jsonMessage);
    }

    public async Task SaveToOutbox<T>(T @event, string key, CancellationToken token = default)
    {
        var topic = KafkaTopicRegistry.GetTopicFor<T>();
        
        var message = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = @event.GetType().Name,
            Key = key,
            Payload = JsonSerializer.Serialize(@event),
            CreatedAt = DateTime.UtcNow,
            Status = OutboxStatus.Pending,
            Topic = topic
        };
        
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        await context.OutboxMessages.AddAsync(message, token);
        await context.SaveChangesAsync(token);
    }
}