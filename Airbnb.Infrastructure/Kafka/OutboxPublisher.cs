using Airbnb.Domain.Entities;
using Airbnb.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Kafka.Interfaces;

namespace Airbnb.Infrastructure.Kafka;

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
        _logger.LogInformation("Outbox publisher starting");
        
        const int batchSize = 25;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var batch = await db.OutboxMessages
                    .Where(x => x.Status == OutboxStatus.Pending)
                    .OrderBy(x => x.CreatedAt)
                    .Take(batchSize)
                    .ToListAsync(stoppingToken);

                if (batch.Count == 0)
                {
                    await Task.Delay(5000, stoppingToken);
                    continue;
                }

                await ProcessOutboxBatchAsync(batch, db, stoppingToken);
                await Task.Delay(5000, stoppingToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug("Outbox publisher cancelled" + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Outbox publisher task crashed" + ex.Message);
            }
        }
    }

    private async Task ProcessOutboxBatchAsync(List<OutboxMessage> batch, AppDbContext db, CancellationToken stoppingToken)
    {
        var tasks = batch.Select(async msg =>
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
                if (msg.RetriesCount >= MaxRetries)
                    msg.Status = OutboxStatus.Failed;
                
                _logger.LogDebug(ex, "Error publishing message {MessageId}", msg.Id);
            }
        });
        
        await Task.WhenAll(tasks);
        
        db.UpdateRange(batch);
        await db.SaveChangesAsync(stoppingToken);
    }
}