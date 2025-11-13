namespace Airbnb.Application.DTO;

public record BalanceResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public decimal Amount { get; init; } 
    public DateTime CreatedAt { get; init; }
    public DateTime? RechargedAt { get; init; }
}