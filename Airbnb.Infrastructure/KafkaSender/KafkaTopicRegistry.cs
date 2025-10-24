using Airbnb.Application.CreatedEvent;
using Contracts.VersionEvents;
using Shared.Kafka.Options;

namespace Airbnb.Infrastructure.KafkaSender;

public class KafkaTopicRegistry
{
    private static readonly Dictionary<Type, string> _map = new()
    {
        { typeof(UserCreatedEvent), KafkaTopics.Users },
        { typeof(UserUpdatedEvent), KafkaTopics.Users },
        { typeof(UserUpdatedEventV1), KafkaTopics.Users}
    };

    public static string GetTopicFor<TEvent>()
    {
        if (_map.TryGetValue(typeof(TEvent), out var topic))
            return topic;
        
        throw new ArgumentException($"Unknown topic {typeof(TEvent).Name}");
    }
}