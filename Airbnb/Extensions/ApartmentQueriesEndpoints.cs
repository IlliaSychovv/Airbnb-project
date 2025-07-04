using Airbnb.Application.DTOs.Dappers;
using Airbnb.Application.Interfaces.Services;

namespace Airbnb.Extensions;

public static class ApartmentQueriesEndpoints
{
    public static void AddMapApartmentsEndpoints(this WebApplication app)
    {
        app.MapPost("apartments/upsert", async (ApartmentUpsertDto dto, IApartmentDapperService service) =>
        {
            await service.UpsertAsync(dto);
            return Results.Ok();
        });

        app.MapGet("/apartments/groupby", async (IApartmentDapperService service) =>
        {
            var result = await service.GetGroupByResultAsync();
            return Results.Ok(result);
        });

        app.MapGet("/apartments/having", async (IApartmentDapperService service) =>
        {
            var result = await service.GetHavingResultsAsync();
            return Results.Ok(result);
        });

        app.MapGet("/apartments/statistics", async (IApartmentDapperService service) =>
        {
            var result = await service.GetStatisticsAsync();
            return Results.Ok(result);
        });

        app.MapGet("/apartments/pricequantiles", async (IApartmentDapperService service) =>
        {
            var result = await service.GetPriceQuantilesAsync();
            return Results.Ok(result);
        });
    }
}