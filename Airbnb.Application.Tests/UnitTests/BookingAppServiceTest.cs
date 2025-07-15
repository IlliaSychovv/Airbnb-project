using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.Services;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Shouldly;
using Moq;

namespace Airbnb.Application.Tests.UnitTests;

public class BookingAppServiceTest
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly BookingAppService _bookingAppService;

    public BookingAppServiceTest()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _bookingServiceMock = new Mock<IBookingService>();
        _bookingAppService = new BookingAppService(_bookingRepositoryMock.Object, _bookingServiceMock.Object);
    }

    [Fact]
    public async Task GetUserBookingsAsync_ShouldReturnUserBookings_WhenWeCallMethod()
    {
        var userId = Guid.NewGuid();
        var bookings = new List<Booking> { new Booking { Id = Guid.NewGuid() } };
        
        _bookingRepositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(bookings);
        
        var result = await _bookingAppService.GetUserBookingsAsync(userId);
        
        result.ShouldBe(bookings);
        _bookingRepositoryMock.Verify(x => x.GetByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturnBookingId_WhenWeCreateBooking()
    {
        var userId = Guid.NewGuid();
        var apartmentId = Guid.NewGuid();
        var dateRange = new DateRange(DateTime.Now, DateTime.Now.AddDays(1));
        var bookings = new Booking { Id = Guid.NewGuid() };
        
        _bookingServiceMock
            .Setup(x => x.CreateBooking(apartmentId, userId, dateRange))
            .ReturnsAsync(bookings);
        
        var result = await _bookingAppService.CreateBookingAsync(userId, apartmentId, dateRange);
        
        result.ShouldBe(bookings.Id);
        _bookingServiceMock.Verify(x => x.CreateBooking(apartmentId, userId, dateRange), Times.Once);
    }
}