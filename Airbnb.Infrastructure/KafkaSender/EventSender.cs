using Airbnb.Application.Interfaces;

namespace Airbnb.Infrastructure.KafkaSender;

public class EventSender : IEventSender
{
    private readonly IKafkaProducer _producer;

    public EventSender(IKafkaProducer producer)
    {
        _producer = producer;
    }

    public async Task SendEvent<T>(string topic, string key, T jsonMessage)
    {
        await _producer.ProduceAsync(topic, key, jsonMessage); 
    }
}