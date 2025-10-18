namespace Airbnb.Domain.Entities;

public class BookingSagaJournal
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public SagaStep Step { get; set; }
    public string? Error { get; set; }
    public string AccountNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum SagaStep
{
    PaymentStarted,
    PaymentFailed,
    BookingCompleted,
    UnexpectedError
}