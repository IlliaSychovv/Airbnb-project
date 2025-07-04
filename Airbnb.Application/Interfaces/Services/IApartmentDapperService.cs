using Airbnb.Application.DTOs.Dappers;

namespace Airbnb.Application.Interfaces.Services;

public interface IApartmentDapperService
{
    Task UpsertAsync(ApartmentUpsertDto dto);
    Task<IEnumerable<GroupByResultDto>> GetGroupByResultAsync();
    Task<IEnumerable<HavingResultDto>> GetHavingResultsAsync();
    Task<AggregateStatsDto> GetStatisticsAsync();
    Task<QuantilesDto> GetPriceQuantilesAsync();
}