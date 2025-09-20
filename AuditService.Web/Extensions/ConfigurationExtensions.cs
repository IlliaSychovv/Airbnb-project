using AuditService.Application.Interfaces;
using AuditService.Infrastructure.Clients;
using AuditService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using Shared.Kafka.Options;

namespace AuditService.Web.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KafkaOptions>(configuration.GetSection("Kafka"));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpClient<IMonolithClient, MonolithClient>(client =>
            {
                var baseUrl = configuration["MonolithApi:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl!);
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
            );
        
        return services;
    }
}