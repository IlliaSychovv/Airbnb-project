using Airbnb.Application.DTO;

namespace Airbnb.Application.Interfaces;

public interface IPaymentClient
{
    Task<bool> TransactionWithdrawAsync(string accountNumber, decimal amount);
    Task<BalanceResponse> GetUserBalanceAsync(Guid userId);
}