using Airbnb.Application.Services;
using Airbnb.Application.Interfaces;
using Airbnb.Infrastructure.Providers;
using Airbnb.Infrastructure.Repositories;
using Airbnb.Infrastructure.Services;
using Airbnb.Application.Interfaces.Providers;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Infrastructure.Kafka;
using Airbnb.Infrastructure.Wrapper;

namespace Airbnb.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserManagerWrapper, UserManagerWrapper>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IApartmentService, ApartmentService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IBookingAppService, BookingAppService>();

        services.AddSingleton<IKafkaProducer>(provider => 
            new KafkaProducer("localhost:9092"));
        
        services.AddSingleton<INpgsqlProvider, NpgsqlProvider>();
        services.AddSingleton<IDbConnectionProvider, DbConnectionProvider>();

        services.AddScoped<IApartmentDapperRepository, ApartmentDapperRepository>();
        services.AddScoped<IApartmentDapperService, ApartmentDapperService>();

        services.AddScoped<BookingService>();
        services.AddScoped<BookingAppService>();
        
        return services;
    }
}