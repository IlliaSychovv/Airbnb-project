namespace PaymentService.Application.DTO;

public record WithdrawDto
{
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}