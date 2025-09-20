using Airbnb.Application.Services;
using Airbnb.Application.Interfaces;
using Airbnb.Infrastructure.Providers;
using Airbnb.Infrastructure.Repositories;
using Airbnb.Infrastructure.Services;
using Airbnb.Application.Interfaces.Providers;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.Options;
using Airbnb.Infrastructure.KafkaSender;
using Airbnb.Infrastructure.Wrapper;
using Microsoft.Extensions.Options;
using Shared.Kafka.Interfaces;
using Shared.Kafka.Kafka;

namespace Airbnb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IApartmentDapperRepository, ApartmentDapperRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<IUserManagerWrapper, UserManagerWrapper>();
        services.AddScoped<IApartmentDapperService, ApartmentDapperService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IApartmentService, ApartmentService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IBookingAppService, BookingAppService>();
        services.AddScoped<IUserService, UserService>();
        
        services.Decorate<IUserRepository, CachedUserRepository>();
        
        services.AddSingleton<IRedisService, RedisService>();
        services.AddSingleton<INpgsqlProvider, NpgsqlProvider>();
        services.AddSingleton<IDbConnectionProvider, DbConnectionProvider>();
        services.AddSingleton<IEventSender, EventSender>();
        services.AddSingleton<IKafkaProducer>(provider =>
        {
            var kafkaOptions = provider.GetRequiredService<IOptions<KafkaOptions>>().Value;
            var logger = provider.GetRequiredService<ILogger<KafkaProducer>>();

            return new KafkaProducer(kafkaOptions.BootstrapServers, logger);
        });
        
        return services;
    }
}