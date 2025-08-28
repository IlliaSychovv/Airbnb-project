using Airbnb.Application.DTO.Dappers;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Services;
using Moq;
using Shouldly;

namespace Airbnb.Application.Tests.UnitTests;

public class ApartmentDapperServiceTest
{
    private readonly Mock<IApartmentDapperRepository> _apartmentDapperRepositoryMock;
    private readonly ApartmentDapperService _apartmentDapperService;

    public ApartmentDapperServiceTest()
    {
        _apartmentDapperRepositoryMock = new Mock<IApartmentDapperRepository>();
        _apartmentDapperService = new ApartmentDapperService(_apartmentDapperRepositoryMock.Object);
    }

    [Fact]
    public async Task Upsert_ShouldReturnsData_WhenWeCallMethod()
    {
        var upsertDto = new ApartmentUpsertDto
        {
            Id = Guid.NewGuid(),
            Title = "testTitle",
            Description = "testDescription",
            Location = "locationTest",
            Price = 100,
            ExternalId = "testExternalId",
            Metadata = "testMetadata"
        };
        
        _apartmentDapperRepositoryMock
            .Setup(x => x.UpsertAsync(It.IsAny<ApartmentUpsertDto>()))
            .Returns(Task.CompletedTask);
        
        await _apartmentDapperService.Upsert(upsertDto);
        
        _apartmentDapperRepositoryMock.Verify(x => x.UpsertAsync(It.IsAny<ApartmentUpsertDto>()), Times.Once);
    }

    [Fact]
    public async Task GetGroupByResult_ShouldReturnsData_WhenWeCallMethod()
    {
        var expected = new List<GroupByResultDto>
        {
            new GroupByResultDto{ Location = "testLocation1", ApartmentCount = 2, AvgPrice = 500},
            new GroupByResultDto{ Location = "testLocation2", ApartmentCount = 3, AvgPrice = 600}
        };
        
        _apartmentDapperRepositoryMock
            .Setup(x => x.GetGroupByResultAsync())
            .ReturnsAsync(expected);
        
        var result = await _apartmentDapperService.GetGroupByResult();
        
        result.ShouldNotBeNull();
        result.First().Location.ShouldBe("testLocation1");
        _apartmentDapperRepositoryMock.Verify(x => x.GetGroupByResultAsync(), Times.Once);
    }

    [Fact]
    public async Task GetHavingResult_ShouldReturnsData_WhenWeCallMethod()
    {
        var expected = new List<HavingResultDto>
        {
            new HavingResultDto{ Location = "testLocation1", ApartmentCount = 2, AvgPrice = 500},
            new HavingResultDto{ Location = "testLocation2", ApartmentCount = 3, AvgPrice = 600}
        };
        
        _apartmentDapperRepositoryMock
            .Setup(x => x.GetHavingResultsAsync())
            .ReturnsAsync(expected);
        
        var result = await _apartmentDapperService.GetHavingResults();
        
        result.ShouldNotBeNull();
        result.First().Location.ShouldBe("testLocation1");
        _apartmentDapperRepositoryMock.Verify(x => x.GetHavingResultsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetStatistics_ShouldReturnsData_WhenWeCallMethod()
    {
        var dto = new AggregateStatsDto
        {
            Count = 2,
            AvgPrice = 500,
            MaxPrice = 1000,
            SumPrice = 400
        };
        
        _apartmentDapperRepositoryMock
            .Setup(x => x.GetStatisticsAsync())
            .ReturnsAsync(dto);
        
        var result = await _apartmentDapperService.GetStatistics();
        
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        _apartmentDapperRepositoryMock.Verify(x => x.GetStatisticsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPriceQuantiles_ShouldReturnsData_WhenWeCallMethod()
    {
        var dto = new QuantilesDto
        {
            Q1 = 4,
            Median = 8,
            Q3 = 12
        };

        _apartmentDapperRepositoryMock
            .Setup(x => x.GetPriceQuantilesAsync())
            .ReturnsAsync(dto);
        
        var result = await _apartmentDapperService.GetPriceQuantiles();
        
        result.ShouldNotBeNull();
        result.Q1.ShouldBe(dto.Q1);
        _apartmentDapperRepositoryMock.Verify(x => x.GetPriceQuantilesAsync(), Times.Once);
    }
}