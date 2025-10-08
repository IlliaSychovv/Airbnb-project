using Mapster;
using PaymentService.Application.DTO;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entity;

namespace PaymentService.Application.Services;

public class BalanceService : IBalanceService
{
    private readonly IBalanceRepository _repository;

    public BalanceService(IBalanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<BalanceDto> GetBalanceByUserId(Guid userId)
    {
        var balance = await _repository.GetBalanceByUserId(userId);
        return balance.Adapt<BalanceDto>();
    }

    public async Task CreateBalance(Guid userId)
    {
        var balance = new Balance
        {
            Id = userId,
            UserId = userId,
            Amount = 0m,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.CreateBalance(balance);
    }
}