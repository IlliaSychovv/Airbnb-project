using AuditService.Application.DTO;
using AuditService.Application.Interfaces;
using AuditService.Infrastructure.Repositories;
using AuditService.Infrastructure.Services;
using AuditService.Web.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Shared.Kafka.Interfaces;
using Shared.Kafka.Kafka;

namespace AuditService.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionHandler, GlobalExceptionHandler>();
        
        services.AddScoped<IAuditService, AuditService.Application.Services.AuditService>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IKafkaMessageHandler<AuditDto>, ProfileKafkaHandler>();

        services.AddHostedService<KafkaConsumer<AuditDto>>();
        
        return services;
    }
}