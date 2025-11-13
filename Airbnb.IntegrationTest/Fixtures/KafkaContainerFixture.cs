using Testcontainers.Kafka;
using Xunit;

namespace Airbnb.IntegrationTest.Fixtures;

public class KafkaContainerFixture : IAsyncLifetime
{
    private KafkaContainer Container { get; set; }
    public string BootstrapServers => Container.GetBootstrapAddress();

    public async Task InitializeAsync()
    {
        Container = new KafkaBuilder()
            .WithImage("confluentinc/cp-kafka:7.6.0")
            .Build();
        
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}