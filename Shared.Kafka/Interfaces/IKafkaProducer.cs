namespace Shared.Kafka.Interfaces;

public interface IKafkaProducer
{
    Task ProduceAsync<T>(string topic, string key, T data);
}