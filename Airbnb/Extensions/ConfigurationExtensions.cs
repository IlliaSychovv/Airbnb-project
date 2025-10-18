using Airbnb.Application.Options;
using Airbnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Redis.Redis;

namespace Airbnb.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisSettingsOption>(
            configuration.GetSection("Redis"));

        services.Configure<KafkaOptions>(
            configuration.GetSection("Kafka"));

        services.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        return services;
    }
}