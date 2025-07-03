using System.Data;
using Airbnb.Application.Interfaces;
using Airbnb.Domain.Entities;
using Airbnb.Application.DTOs.Dappers;
using Dapper;
using Mapster;

namespace Airbnb.Infrastructure.Services;

public class ApartmentDapperService : IApartmentDapperService
{
    private readonly IDbConnection _connection;
    private readonly INpgsqlProvider _npgsqlProvider;

    public ApartmentDapperService(IDbConnection connection, INpgsqlProvider npgsqlProvider)
    {
        _connection = connection;
        _npgsqlProvider = npgsqlProvider;
    }

    public async Task UpsertAsync(Apartment apartment)
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/Upsert.sql");

        var parameters = apartment.Adapt<ApartmentUpsertDto>();
        
        await _connection.ExecuteAsync(sql, parameters);
    }

    public async Task<IEnumerable<GroupByResultDto>> GetGroupByResultAsync()
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/GroupByResult.sql");
        return await _connection.QueryAsync<GroupByResultDto>(sql);
    }

    public async Task<IEnumerable<HavingResultDto>> GetHavingResultsAsync()
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/HavingResults.sql");
        return await _connection.QueryAsync<HavingResultDto>(sql);
    }

    public async Task<AggregateStatsDto> GetStatisticsAsync()
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/Statistics.sql");
        return await _connection.QuerySingleAsync<AggregateStatsDto>(sql);
    }

    public async Task<QuantilesDto> GetPriceQuantilesAsync()
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/GetPriceQuantiles.sql");
        return await _connection.QuerySingleAsync<QuantilesDto>(sql);
    }
}