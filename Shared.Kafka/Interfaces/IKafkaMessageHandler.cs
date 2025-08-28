namespace Shared.Kafka.Interfaces;

public interface IKafkaMessageHandler<T>
{
    Task HandleMessage(string message, CancellationToken cancellationToken);
}