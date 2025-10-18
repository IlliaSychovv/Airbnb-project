using Airbnb.Application.DTO.Dappers;

namespace Airbnb.Application.Interfaces.Services;

public interface IApartmentDapperService
{
    Task Upsert(ApartmentUpsertDto dto);
    Task<IEnumerable<GroupByResultDto>> GetGroupByResult();
    Task<IEnumerable<HavingResultDto>> GetHavingResults();
    Task<AggregateStatsDto> GetStatistics();
    Task<QuantilesDto> GetPriceQuantiles();
}