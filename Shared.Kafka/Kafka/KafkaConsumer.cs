using Shared.Kafka.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Kafka.Options;

namespace Shared.Kafka.Kafka;

public class KafkaConsumer<T> : BackgroundService
{
     private readonly IServiceProvider _serviceProvider;
     private readonly ILogger<KafkaConsumer<T>> _logger;
     private readonly KafkaOptions _options;

     public KafkaConsumer(IServiceProvider serviceProvider, IOptions<KafkaOptions> options,
          ILogger<KafkaConsumer<T>> logger)
     {
          _serviceProvider = serviceProvider;
          _logger = logger;
          _options = options.Value;
     }
     
     protected override Task ExecuteAsync(CancellationToken stoppingToken)
     {
          return StartKafka(stoppingToken); 
     }
     
     private async Task StartKafka(CancellationToken token)
     {
          var config = new ConsumerConfig
          {
               BootstrapServers = _options.BootstrapServers,
               GroupId = _options.GroupId,
               AutoOffsetReset = AutoOffsetReset.Earliest
          };
          
          var consumer = new ConsumerBuilder<string, string>(config).Build(); 
          consumer.Subscribe(new[] { KafkaTopics.Users});
     
          try
          {
               while (!token.IsCancellationRequested)
               {
                    var result = consumer.Consume(token);
                    
                    using var scope = _serviceProvider.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IKafkaMessageHandler<T>>();
                      
                    await handler.HandleMessage(result.Message.Value, token);
               }
          }
          catch (OperationCanceledException)
          {
               _logger.LogInformation("Kafka consumer canceled");
               consumer.Close();
          }
          catch (Exception ex)
          {
               _logger.LogError(ex, "Kafka consumer stopped");
          }
     }
}