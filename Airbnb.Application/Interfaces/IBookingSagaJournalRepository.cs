using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces;

public interface IBookingSagaJournalRepository
{
    Task AddSagaAsync(Guid bookingId, SagaStep step);
    Task UpdateSagaAsync(Guid bookingId, SagaStep step, string? error = null);
}