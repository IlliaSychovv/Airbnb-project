using Airbnb.Application.Interfaces;
using Confluent.Kafka;

namespace Airbnb.Infrastructure.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;

    public KafkaProducer(string bootstrapServers)
    {
        var config = new ProducerConfig{ BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string data)
    {
        try
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = data });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Kafka error: " + ex);
        }
    }
}