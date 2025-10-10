using Airbnb.Domain.ValueObject;

namespace Airbnb.Application.Interfaces;

public interface IBookingSagaOrchestrator
{
    Task<Guid?> CreateSagaBooking(Guid apartmentId, Guid userId, DateRange range, decimal price, string accountNumber);
}