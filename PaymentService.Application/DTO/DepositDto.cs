namespace PaymentService.Application.DTO;

public record DepositDto
{
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}