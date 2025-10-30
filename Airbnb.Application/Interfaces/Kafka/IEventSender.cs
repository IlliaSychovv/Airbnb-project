namespace Airbnb.Application.Interfaces.Kafka;

public interface IEventSender
{ 
    Task SendEvent<TEvent>(object key, TEvent message);
    Task SaveToOutbox<T>(T @event, string key, CancellationToken token = default);
}