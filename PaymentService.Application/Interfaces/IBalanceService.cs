using PaymentService.Application.DTO;

namespace PaymentService.Application.Interfaces;

public interface IBalanceService
{
    Task<BalanceDto> GetBalanceByUserId(Guid userId);
    Task CreateBalance(Guid userId);
}