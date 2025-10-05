using Airbnb.Domain.Entities;
using Airbnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Kafka.Interfaces;

namespace Airbnb.Infrastructure.KafkaSender;

public class OutboxPublisher : BackgroundService
{
    private const int MaxRetries = 3;
    private readonly IServiceProvider _serviceProvider;
    private readonly IKafkaProducer _producer;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(IServiceProvider serviceProvider, IKafkaProducer producer, ILogger<OutboxPublisher> logger)
    {
        _serviceProvider = serviceProvider;
        _producer = producer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("Outbox publisher starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var message = await db.OutboxMessages
                    .Where(x => x.Status == OutboxStatus.Pending)
                    .OrderBy(x => x.CreatedAt)
                    .Take(25)
                    .ToListAsync(stoppingToken);

                foreach (var msg in message)
                {
                    try
                    {
                        await _producer.ProduceAsync(msg.Topic, msg.Key, msg.Payload);

                        msg.Status = OutboxStatus.Sent;
                        msg.ProcessedAt = DateTime.UtcNow;
                        
                        _logger.LogDebug("Message {MessageId} published to Kafka (Topic={Topic}, Key={Key})", 
                            msg.Id, msg.Topic, msg.Key);
                    }
                    catch (Exception ex)
                    {
                        msg.RetriesCount++;
                        _logger.LogDebug(ex.Message + "Error publishing message {MessageId} (retry {RetriesCount})", msg.Id, msg.RetriesCount);

                        if (msg.RetriesCount >= MaxRetries)
                        {
                            msg.Status = OutboxStatus.Failed;
                            msg.ProcessedAt = DateTime.UtcNow;
                            _logger.LogWarning("Message {MessageId} marked as 'Failed'", msg.Id);
                        }
                    }
                }
                
                await db.SaveChangesAsync(stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug("Outbox publisher task cancelled:" + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Outbox publisher task crashed:" + ex.Message);
            }
        }
    }
}