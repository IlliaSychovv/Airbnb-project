using Airbnb.Application.DTOs.Dappers;
using Airbnb.Application.Interfaces.Repositories;
using Airbnb.Application.Interfaces.Services;

namespace Airbnb.Application.Services;

public class ApartmentDapperService : IApartmentDapperService
{
    private readonly IApartmentDapperRepository _apartmentDapperRepository;

    public ApartmentDapperService(IApartmentDapperRepository apartmentDapperRepository)
    {
        _apartmentDapperRepository = apartmentDapperRepository;
    }

    public async Task Upsert(ApartmentUpsertDto dto)
    {
        await _apartmentDapperRepository.UpsertAsync(dto);
    }

    public async Task<IEnumerable<GroupByResultDto>> GetGroupByResult()
    {
        return await _apartmentDapperRepository.GetGroupByResultAsync();
    }

    public async Task<IEnumerable<HavingResultDto>> GetHavingResults()
    {
        return await _apartmentDapperRepository.GetHavingResultsAsync();
    }

    public async Task<AggregateStatsDto> GetStatistics()
    {
        return await _apartmentDapperRepository.GetStatisticsAsync();
    }

    public async Task<QuantilesDto> GetPriceQuantiles()
    {
        return await _apartmentDapperRepository.GetPriceQuantilesAsync();
    }
}