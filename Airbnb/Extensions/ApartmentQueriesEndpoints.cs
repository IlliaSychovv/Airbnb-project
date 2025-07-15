using Airbnb.Application.DTOs.Dappers;
using Airbnb.Application.Interfaces.Services;

namespace Airbnb.Extensions;

public static class ApartmentQueriesEndpoints
{
    public static void AddMapApartmentsEndpoints(this WebApplication app)
    {
        var apartmentsGroup = app.MapGroup("/apartments");

        apartmentsGroup.MapGet("/groupby", async (IApartmentDapperService service) =>
        {
            var result = await service.GetGroupByResult();
            return Results.Ok(result);
        });

        apartmentsGroup.MapGet("/having", async (IApartmentDapperService service) =>
        {
            var result = await service.GetHavingResults();
            return Results.Ok(result);
        });

        apartmentsGroup.MapGet("/statistics", async (IApartmentDapperService service) =>
        {
            var result = await service.GetStatistics();
            return Results.Ok(result);
        });

        apartmentsGroup.MapGet("/pricequantiles", async (IApartmentDapperService service) =>
        {
            var result = await service.GetPriceQuantiles();
            return Results.Ok(result);
        });
        
        apartmentsGroup.MapPost("/upsert", async (ApartmentUpsertDto dto, IApartmentDapperService service) =>
        {
            await service.Upsert(dto);
            return Results.Ok();
        });
    }
}