using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.Services;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Moq;
using Shouldly;

namespace Airbnb.Application.Tests.UnitTests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _bookingService = new BookingService(_bookingRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateBooking_ShouldReturnBooking_WhenWeCallThisMethod()
    {
        var userId = Guid.NewGuid();
        var apartmentId = Guid.NewGuid();
        var dateRange = new DateRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        
        _bookingRepositoryMock
            .Setup(x => x.ExistsConflictAsync(apartmentId, dateRange))
            .ReturnsAsync(false);
        
        _bookingRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);
        
        var result = await _bookingService.CreateBooking(userId, apartmentId, dateRange);
        
        result.ShouldNotBeNull();
        _bookingRepositoryMock.Verify(x => x.ExistsConflictAsync(apartmentId, dateRange), Times.Once);
        _bookingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Booking>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_ShouldReturnException_WhenWeCreateBookingWithInvalidData()
    {
        var userId = Guid.NewGuid();
        var apartmentId = Guid.NewGuid();
        var dateRange = new DateRange(DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
        
        _bookingRepositoryMock
            .Setup(x => x.ExistsConflictAsync(apartmentId, dateRange))
            .ReturnsAsync(true);
        
        await Should.ThrowAsync<InvalidOperationException>(
            () => _bookingService.CreateBooking(userId, apartmentId, dateRange));
        
        _bookingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Booking>()), Times.Never);
    }
}