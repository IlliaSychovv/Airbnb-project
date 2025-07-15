using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.DTOs;
using Airbnb.Application.Services;
using Airbnb.Domain.Entities;
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
        string location = null;
        
        var apartment = new List<Apartment>()
        {
            new Apartment { Title = "testTitle1", Description = "testDescription1", Location = "testLocation1", Price = 100 },
            new Apartment() { Title = "testTitle2", Description = "testDescription2", Location = "testLocation2", Price = 200 }
        };
        
        _apartmentRepository
            .Setup(x => x.GetAsync(pageNumber, pageSize, location))
            .ReturnsAsync(apartment);

        _apartmentRepository
            .Setup(x => x.GetTotalCountAsync(location))
            .ReturnsAsync(10);
        
        var result = await _apartmentService.GetPagedApartmentsAsync(pageNumber, pageSize, location);
        
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(10);
        result.PageSize.ShouldBe(5);
        _apartmentRepository.Verify(x => x.GetAsync(pageNumber, pageSize, location), Times.Once);
    }
}