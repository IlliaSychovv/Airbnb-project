using Airbnb.Application.DTOs.Dappers;
using Airbnb.Application.Interfaces.Providers;
using Airbnb.Application.Interfaces.Repositories;
using Dapper;

namespace Airbnb.Infrastructure.Repositories;

public class ApartmentDapperRepository : IApartmentDapperRepository
{
    private readonly IDbConnectionProvider _connectionProvider;
    private readonly INpgsqlProvider _npgsqlProvider;

    public ApartmentDapperRepository(IDbConnectionProvider connectionProvider, INpgsqlProvider npgsqlProvider)
    {
        _connectionProvider = connectionProvider;
        _npgsqlProvider = npgsqlProvider;
    }

    public async Task UpsertAsync(ApartmentUpsertDto dto)
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/Upsert.sql");
        using var connection = _connectionProvider.CreateConnection();
        await connection.ExecuteAsync(sql, dto);
    }

    public async Task<IEnumerable<GroupByResultDto>> GetGroupByResultAsync()
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/GroupByResult.sql");
        using var connection = _connectionProvider.CreateConnection();
        return await connection.QueryAsync<GroupByResultDto>(sql);
    }

    public async Task<IEnumerable<HavingResultDto>> GetHavingResultsAsync()
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/HavingResult.sql");
        using var connection = _connectionProvider.CreateConnection();
        return await connection.QueryAsync<HavingResultDto>(sql);
    }

    public async Task<AggregateStatsDto> GetStatisticsAsync()
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/Statistics.sql");
        using var connection = _connectionProvider.CreateConnection();
        return await connection.QuerySingleAsync<AggregateStatsDto>(sql);
    }

    public async Task<QuantilesDto> GetPriceQuantilesAsync()
    {
        var sql = _npgsqlProvider.Get("ResourcesSql/Apartment/GetPriceQuantiles.sql");
        using var connection = _connectionProvider.CreateConnection();
        return await connection.QuerySingleAsync<QuantilesDto>(sql);
    }
}