using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces.SagaOrchestrator;

public interface IBookingSagaOrchestrator
{
    Task<Guid?> CreateSagaBooking(Guid apartmentId, Guid userId, DateRange range, decimal price, string accountNumber);
    Task RecoveryStuckSaga(BookingSagaJournal saga);
}