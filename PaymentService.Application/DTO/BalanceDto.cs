namespace PaymentService.Application.DTO;

public record BalanceDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RechargedAt { get; set; }
}