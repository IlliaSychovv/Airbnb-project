using Airbnb.Application.BookingOrchestrator;
using Airbnb.Application.Services;
using Airbnb.Application.Interfaces;
using Airbnb.Infrastructure.Providers;
using Airbnb.Infrastructure.Repositories;
using Airbnb.Infrastructure.Services;
using Airbnb.Application.Interfaces.Providers;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.Options;
using Airbnb.Domain.Entities;
using Airbnb.Infrastructure.Client;
using Airbnb.Infrastructure.KafkaSender;
using Airbnb.Infrastructure.RedisServices;
using Airbnb.Infrastructure.Wrapper;
using Microsoft.Extensions.Options;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Shared.Kafka.Interfaces;
using Shared.Kafka.Kafka;
using StackExchange.Redis;

namespace Airbnb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IApartmentDapperRepository, ApartmentDapperRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<IBookingSagaOrchestrator, BookingSagaOrchestrator>();
        services.AddScoped<IBookingSagaJournalRepository, BookingSagaJournalRepository>();
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
        services.AddSingleton<IRedisLock>(sp =>
        {
            var redisOptions = sp.GetRequiredService<IOptions<RedisSettingsOption>>().Value;
            var muxer = ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
            var redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { muxer });

            return new RedisLock(redLockFactory);
        });  
        services.AddSingleton<IKafkaProducer>(provider =>
        {
            var kafkaOptions = provider.GetRequiredService<IOptions<KafkaOptions>>().Value;
            var logger = provider.GetRequiredService<ILogger<KafkaProducer>>();

            return new KafkaProducer(kafkaOptions.BootstrapServers, logger);
        });

        services.AddHostedService<OutboxPublisher>();

        services.AddHttpClient<IPaymentClient, PaymentClient>(client =>
        {
            var baseUrl = configuration["PaymentMicroserviceApi:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl!);
        });
        
        return services;
    }
}