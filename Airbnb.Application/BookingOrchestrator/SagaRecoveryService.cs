using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.SagaOrchestrator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Airbnb.Application.BookingOrchestrator;

public class SagaRecoveryService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SagaRecoveryService> _logger;

    public SagaRecoveryService(IServiceScopeFactory scopeFactory, ILogger<SagaRecoveryService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var orchestrator = scope.ServiceProvider.GetRequiredService<IBookingSagaOrchestrator>();
            var repository = scope.ServiceProvider.GetRequiredService<IBookingSagaJournalRepository>();
            
            var stuckSaga = await repository.GetStuckSagasAsync();
            _logger.LogInformation("Found {Count} stuck sagas", stuckSaga.Count);

            foreach (var saga in stuckSaga)
            {
                try
                {
                    await orchestrator.RecoveryStuckSaga(saga);
                    _logger.LogInformation($"Recovery executed for BookingId: {saga.BookingId}");
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, $"Error during recovery for BookingId: {saga.BookingId}");
                }
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}