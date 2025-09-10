using System.Text.Json;
using Airbnb.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Shared.Kafka.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(string bootstrapServers, ILogger<KafkaProducer> logger)
    {
        var config = new ProducerConfig{ BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<string, string>(config).Build();
        _logger = logger;
    }

    public async Task ProduceAsync<T>(string topic, string key, T data)
    {
        try
        { 
            string json;
            if (data is string str)
                json = str;
            else
                json = JsonSerializer.Serialize(data);
            
            var message = new Message<string, string> { Key = key, Value = json };

            await _producer.ProduceAsync(topic, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while producing message to Kafka");
        }
    }
}