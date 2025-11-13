using Testcontainers.Redis;
using Xunit;

namespace Airbnb.IntegrationTest.Fixtures;

public class RedisContainerFixture : IAsyncLifetime
{
    private RedisContainer Container { get; set; }
    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        Container = new RedisBuilder()
            .WithImage("redis:7.2-alpine")
            .Build();
        
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}