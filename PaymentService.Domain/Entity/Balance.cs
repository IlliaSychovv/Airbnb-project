namespace PaymentService.Domain.Entity;

public class Balance
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? RechargedAt { get; set; }
}