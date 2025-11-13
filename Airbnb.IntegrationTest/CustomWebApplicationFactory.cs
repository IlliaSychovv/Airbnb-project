using Airbnb.Application.Interfaces;
using Airbnb.Application.Options;
using Airbnb.Infrastructure.Data;
using Airbnb.IntegrationTest.Mocks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Redis.Redis;

namespace Airbnb.IntegrationTest;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private string _postgresConnection;
    private string _redisConnection;
    private string _kafkaConnection;

    public CustomWebApplicationFactory(string postgresConnection, string redisConnection, string kafkaConnection)
    {
        _postgresConnection = postgresConnection;
        _redisConnection = redisConnection;
        _kafkaConnection = kafkaConnection;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(async service =>
        {
            var descriptor = service.SingleOrDefault
                (descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                service.Remove(descriptor);
            
            service.AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(_postgresConnection),
                ServiceLifetime.Scoped 
            );
            
            service.Configure<RedisSettingsOption>(opt => 
                opt.ConnectionString = _redisConnection);
            
            service.Configure<KafkaOptions>(opt => 
                opt.BootstrapServers = _kafkaConnection);

            var paymentDescriptor = service.SingleOrDefault
                (descriptor => descriptor.ServiceType == typeof(IPaymentClient));
            if (paymentDescriptor != null)
                service.Remove(paymentDescriptor);

            service.AddSingleton<IPaymentClient, FakePaymentClient>();
            
            var sp = service.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureDeleted();
            context.Database.Migrate();
            
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var roles = new[] { "Client", "Host", "Admin" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));   
                }
            }
        });
    }
}