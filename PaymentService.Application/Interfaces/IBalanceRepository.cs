using PaymentService.Domain.Entity;

namespace PaymentService.Application.Interfaces;

public interface IBalanceRepository
{
    Task<Balance?> GetBalanceByUserId(Guid userId);
    Task CreateBalance(Balance balance);
}