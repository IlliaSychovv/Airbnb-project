using System.Threading.Tasks;
using System.Net.Http.Json;
using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Authorization;
using Airbnb.IntegrationTest.Fixtures;
using Shouldly;
using Xunit;

namespace Airbnb.IntegrationTest.Tests;

public class RegistrationTest : IClassFixture<PostgresContainersFixture>, 
    IClassFixture<KafkaContainerFixture>, IClassFixture<RedisContainerFixture>
{
    private readonly HttpClient _client;

    public RegistrationTest(PostgresContainersFixture postgres, KafkaContainerFixture kafka,
        RedisContainerFixture redis)
    {
        var factory = new CustomWebApplicationFactory(
            postgres.ConnectionString,
            redis.ConnectionString,
            kafka.BootstrapServers
        );
        
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegistrationTestAsync()
    {
        var dto = new
        {
            Name = "Name", 
            Email = "someTest@gmail.com",
            Password = "2345643Qq!1", 
            PhoneNumber = "+86421956789",
            Role = "Client"
        };
        var registerResponse = await _client.PostAsJsonAsync("api/v1/auth/register", dto);
        registerResponse.EnsureSuccessStatusCode();
        
        var response = await registerResponse.Content.ReadFromJsonAsync<RegisterResponseDto>();
        var userId = response!.UserId;
        
        var paymentResponse = await _client.GetAsync($"api/v1/users/balance/{userId}");
        paymentResponse.EnsureSuccessStatusCode();
        
        var balance = await paymentResponse.Content.ReadFromJsonAsync<BalanceResponse>();
        
        balance.ShouldNotBeNull();
        balance.Amount.ShouldBe(0);
    }
}