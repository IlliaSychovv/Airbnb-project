using Airbnb.Application.DTOs.Dappers;
using Airbnb.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;

namespace Airbnb.Extensions;

public static class ApartmentQueriesEndpoints
{
    public static void AddMapApartmentsQueriesEndpoints(this WebApplication app)
    {
        var apartmentsGroup = app.MapGroup("/apartment")
            .RequireAuthorization(new AuthorizeAttribute
            { 
                Roles = "Admin"
            });

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