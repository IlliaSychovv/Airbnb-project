using System.Text.Json;
using AuditService.Domain.Entity;
using AuditService.Infrastructure.Data;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuditService.Infrastructure.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaConsumerService> _logger;

    public KafkaConsumerService(IServiceProvider serviceProvider, IConfiguration configuration,
        ILogger<KafkaConsumerService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() => StartKafka(stoppingToken), stoppingToken);
        return Task.CompletedTask;
    }

    private void StartKafka(CancellationToken token)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = _configuration["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_configuration["Kafka:Topic"]);

        try
        {
            while (!token.IsCancellationRequested)
            {
                var result = consumer.Consume(token);

                if (result != null)
                {
                    var kafkaMessage = result.Message.Value;
                    _logger.LogInformation("Kafka message received: {Message}", kafkaMessage);

                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var json = JsonDocument.Parse(kafkaMessage);
                    var userId = Guid.Parse(json.RootElement.GetProperty("Id").GetString()!);

                    var entity = new Audit
                    {
                        CreatedAt = result.Timestamp.UtcDateTime,
                        UserId = userId,
                        ProfileJson = kafkaMessage
                    };

                    db.Audits.Add(entity);
                    db.SaveChanges();
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Kafka consumer cancelled");
            consumer.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Kafka consumer");
        }
    }
}