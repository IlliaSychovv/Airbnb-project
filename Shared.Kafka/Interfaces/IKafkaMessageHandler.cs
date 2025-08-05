namespace Airbnb.Application.Interfaces;

public interface IKafkaMessageHandler<T>
{
    Task HandleMessage(T message, CancellationToken cancellationToken);
}