using Airbnb.Application.DTO;
using Airbnb.Application.Interfaces;

namespace Airbnb.IntegrationTest.Mocks;

public class FakePaymentClient : IPaymentClient
{
    public async Task<bool> TransactionWithdrawAsync(string accountNumber, decimal amount)
    {
        return await Task.FromResult(true);
    }

    public async Task<BalanceResponse> GetUserBalanceAsync(Guid userId)
    {
        return await Task.FromResult(new BalanceResponse
        {
            Id = userId,
            UserId = userId,
            Amount = 0m,
            CreatedAt = DateTime.UtcNow,
            RechargedAt = null
        });
    }
}