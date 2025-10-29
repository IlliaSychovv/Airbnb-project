using AuditService.Application.DTO;
using AuditService.Application.Interfaces;
using AuditService.Infrastructure.Repositories;
using AuditService.Infrastructure.Services;
using AuditService.Web.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using Shared.Kafka.Interfaces;
using Shared.Kafka.Kafka;
using Shared.Kafka.Options;
using Shared.Kafka.Topics;

namespace AuditService.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();
        
        services.AddScoped<IAuditService, AuditService.Application.Services.AuditService>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IKafkaMessageHandler<AuditUserDto>, ProfileKafkaHandler>();
        services.AddScoped<IKafkaMessageHandler<AuditApartmentDto>, ApartmentKafkaHandler>();

        services.AddHostedService<KafkaConsumer<AuditUserDto>>(provider => 
            new KafkaConsumer<AuditUserDto>(
                provider,
                provider.GetRequiredService<IOptions<KafkaOptions>>(),
                provider.GetRequiredService<ILogger<KafkaConsumer<AuditUserDto>>>(),
                new[] { KafkaTopics.Users }  
            )
        );
        
        services.AddHostedService<KafkaConsumer<AuditApartmentDto>>(provider => 
            new KafkaConsumer<AuditApartmentDto>(
                provider,
                provider.GetRequiredService<IOptions<KafkaOptions>>(),
                provider.GetRequiredService<ILogger<KafkaConsumer<AuditApartmentDto>>>(),
                new[] { KafkaTopics.Apartments }  
            )
        );
        
        return services;
    }
}