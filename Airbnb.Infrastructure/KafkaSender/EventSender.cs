using System.Text.Json;
using Airbnb.Application.Interfaces;

namespace Airbnb.Infrastructure.KafkaSender;

public class EventSender : IEventSender
{
    private readonly IKafkaProducer _producer;

    public EventSender(IKafkaProducer producer)
    {
        _producer = producer;
    }

    public async Task SendEvent<TEvent>(object key, TEvent message)
    {
        var topic = KafkaTopicRegistry.GetTopicFor<TEvent>();
        
        var jsonMessage = JsonSerializer.Serialize(message);
        
        await _producer.ProduceAsync(topic, key.ToString(), jsonMessage);
    }
}