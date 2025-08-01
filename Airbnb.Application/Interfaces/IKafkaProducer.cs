namespace Airbnb.Application.Interfaces;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string data);
}