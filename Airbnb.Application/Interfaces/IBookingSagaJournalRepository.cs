using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces;

public interface IBookingSagaJournalRepository
{
    Task AddSagaAsync(Guid bookingId, SagaStep step, string accountNumber, decimal amount);
    Task UpdateSagaAsync(Guid bookingId, SagaStep step, string? error = null);
    Task<List<BookingSagaJournal>> GetStuckSagasAsync();
}