namespace Airbnb.Application.Interfaces.Providers;

public interface IBookingDateRangeLockProvider
{
    Task LockDateRangeForBooking(Guid apartmentId, DateTime startDate, DateTime endDate);
}