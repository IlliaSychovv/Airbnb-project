using Airbnb.Application.DTO;
using Airbnb.Application.DTO.Pagination;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Services;
using Airbnb.Domain.Entities;
using Airbnb.Domain.ValueObject;
using Moq;
using Shouldly;

namespace Airbnb.Application.Tests.UnitTests;

public class ApartmentServiceTest
{
    private readonly Mock<IApartmentRepository> _apartmentRepository;
    private readonly ApartmentService _apartmentService;

    public ApartmentServiceTest()
    {
        _apartmentRepository = new Mock<IApartmentRepository>();
        _apartmentService = new ApartmentService(_apartmentRepository.Object);
    }

    [Fact]
    public async Task CreateApartmentAsync_ShouldReturnApartment_WhenWeCreateApartment()
    {
        var apartmentDto = new CreateApartmentDto
        {
            Title = "testTitle",
            Description = "testDescription",
            Location = "testLocation",
            Price = 100
        };

        _apartmentRepository
            .Setup(x => x.AddAsync(It.IsAny<Apartment>()))
            .Returns(Task.CompletedTask);
        
        var result = await _apartmentService.CreateApartmentAsync(apartmentDto);
        
        result.ShouldNotBeNull();
        _apartmentRepository.Verify(x => x.AddAsync(It.IsAny<Apartment>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedApartmentsAsync_ShouldReturnPagedApartmentsList_WhenWeCallMethod()
    {
        int pageNumber = 1;
        int pageSize = 5;
        int totalCount = 1;
        string location = null;
        
        var apartments = new PagedResult<Apartment>
        {
            Items = new List<Apartment>(),
            TotalCount = totalCount
        };
        
        _apartmentRepository
            .Setup(x => x.GetAllApartmentsAsync(pageNumber, pageSize, location))
            .ReturnsAsync(apartments);
        
        var result = await _apartmentService.GetPagedApartmentsAsync(pageNumber, pageSize, location);
        
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(1);
        result.PageSize.ShouldBe(5);
        _apartmentRepository.Verify(x => x.GetAllApartmentsAsync(pageNumber, pageSize, location), Times.Once);
    }

    [Fact]
    public async Task GetAllApartmentsWithBookings_ShouldReturnPagedList_WhenWeCallMethod()
    {
        int pageNumber = 1;
        int pageSize = 5;
        int totalCount = 1;

        var apartments = new PagedResult<Apartment>
        {
            Items = new List<Apartment>(),
            TotalCount = totalCount
        };
        
        _apartmentRepository
            .Setup(x => x.GetAllApartmentsWithBookingsAsync(pageNumber, pageSize))
            .ReturnsAsync(apartments);
        
        var result = await _apartmentService.GetAllApartmentsWithBookings(pageNumber, pageSize);
        
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(1);
        result.PageSize.ShouldBe(5);
        _apartmentRepository.Verify(x => x.GetAllApartmentsWithBookingsAsync(pageNumber, pageSize), Times.Once);
    }

    [Fact]
    public async Task GetAvailableApartments_ShouldReturnPagedList_WhenWeCallMethod()
    {
        int pageNumber = 1;
        int pageSize = 5;
        int totalCount = 1;
        
        decimal minPrice = 100;
        decimal maxPrice = 200;
        
        DateTime startDate = new DateTime(2020, 1, 1);
        DateTime endDate = new DateTime(2020, 11, 10);
        DateRange range = new DateRange(startDate, endDate);

        var apartments = new PagedResult<Apartment>
        {
            Items = new List<Apartment>(),
            TotalCount = totalCount
        };

        _apartmentRepository
            .Setup(x => x.GetAvailableApartmentsAsync(range, pageNumber, pageSize, minPrice, maxPrice))
            .ReturnsAsync(apartments);
        
        var result = await _apartmentService.GetAvailableApartments(range, pageNumber, pageSize, minPrice, maxPrice);
        
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(1);
        result.PageSize.ShouldBe(5);
        _apartmentRepository.Verify(x => x.GetAvailableApartmentsAsync(range, pageNumber, pageSize, minPrice, maxPrice), Times.Once);
    }
}