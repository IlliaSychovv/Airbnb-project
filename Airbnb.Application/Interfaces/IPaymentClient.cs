namespace Airbnb.Application.Interfaces;

public interface IPaymentClient
{
    Task<bool> TransactionWithdrawAsync(string accountNumber, decimal amount);
}