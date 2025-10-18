using Airbnb.Application.DTO.Dappers;

namespace Airbnb.Application.Interfaces.Repositories;

public interface IApartmentDapperRepository
{
    Task UpsertAsync(ApartmentUpsertDto dto);
    Task<IEnumerable<GroupByResultDto>> GetGroupByResultAsync();
    Task<IEnumerable<HavingResultDto>> GetHavingResultsAsync();
    Task<AggregateStatsDto> GetStatisticsAsync();
    Task<QuantilesDto> GetPriceQuantilesAsync();
}