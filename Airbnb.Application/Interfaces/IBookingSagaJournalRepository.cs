using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces;

public interface IBookingSagaJournalRepository
{
    Task AddStepAsync(Guid bookingId, SagaStep step, SagaStepStatus status);
    Task UpdateStepStatusAsync(Guid bookingId, SagaStep step, SagaStepStatus status);
}