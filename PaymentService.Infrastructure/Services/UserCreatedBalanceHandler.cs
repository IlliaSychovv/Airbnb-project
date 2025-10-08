using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Event;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entity;
using PaymentService.Infrastructure.Data;
using Shared.Kafka.Interfaces;

namespace PaymentService.Infrastructure.Services;

public class UserCreatedBalanceHandler : IKafkaMessageHandler<UserCreatedEvent>
{
    private readonly IBalanceService _balanceService;
    private readonly ILogger<UserCreatedBalanceHandler> _logger;

    public UserCreatedBalanceHandler(IBalanceService balanceService, ILogger<UserCreatedBalanceHandler> logger)
    {
        _balanceService = balanceService;
        _logger = logger;
    }

    public async Task HandleMessage(string message, string key, CancellationToken cancellationToken)
    {
        var userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message);
        if (userEvent == null)
            return;

        await _balanceService.CreateBalance(userEvent.Id);
        
        _logger.LogInformation("Balance created for UserId {UserId}", userEvent.Id);
    }
}