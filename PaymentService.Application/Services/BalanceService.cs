using Mapster;
using PaymentService.Application.DTO;
using PaymentService.Application.Interfaces;

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
}