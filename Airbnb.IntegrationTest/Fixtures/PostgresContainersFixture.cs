using Testcontainers.PostgreSql;
using Xunit;

namespace Airbnb.IntegrationTest.Fixtures;

public class PostgresContainersFixture : IAsyncLifetime
{
    private PostgreSqlContainer Container { get; set; }
    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder()
            .WithDatabase("testDb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
        
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}