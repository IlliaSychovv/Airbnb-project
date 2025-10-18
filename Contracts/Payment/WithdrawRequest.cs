namespace Contracts.Payment;

public record WithdrawRequest
{
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}