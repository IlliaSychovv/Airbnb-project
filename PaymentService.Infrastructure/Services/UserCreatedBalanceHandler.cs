using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Event;
using PaymentService.Domain.Entity;
using PaymentService.Infrastructure.Data;
using Shared.Kafka.Interfaces;

namespace PaymentService.Infrastructure.Services;

public class UserCreatedBalanceHandler : IKafkaMessageHandler<UserCreatedEvent>
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserCreatedBalanceHandler> _logger;

    public UserCreatedBalanceHandler(AppDbContext context, ILogger<UserCreatedBalanceHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task HandleMessage(string message, string key, CancellationToken cancellationToken)
    {
        var userEvent = JsonSerializer.Deserialize<UserCreatedEvent>(message);
        if (userEvent == null)
            return;

        if (await _context.Balances.AnyAsync(x => x.UserId == userEvent.Id, cancellationToken))
            return;

        var balance = new Balance
        {
            Id = userEvent.Id,
            UserId = userEvent.Id,
            Amount = 0m,
            CreatedAt = DateTime.UtcNow
        };
        
        await _context.Balances.AddAsync(balance, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Balance created for UserId {UserId}", userEvent.Id);
    }
}