using Airbnb.Application.DTOs.Dappers;
using Airbnb.Domain.Entities;

namespace Airbnb.Application.Interfaces;

public interface IApartmentDapperService
{
    Task UpsertAsync(Apartment apartment);
    Task<IEnumerable<GroupByResultDto>> GetGroupByResultAsync();
    Task<IEnumerable<HavingResultDto>> GetHavingResultsAsync();
    Task<AggregateStatsDto> GetStatisticsAsync();
    Task<QuantilesDto> GetPriceQuantilesAsync();
}