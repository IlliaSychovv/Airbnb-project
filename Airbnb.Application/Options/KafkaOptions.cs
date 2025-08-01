namespace Airbnb.Application.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; set; } = default!;
    public string Topic { get; set; } = default!;
}