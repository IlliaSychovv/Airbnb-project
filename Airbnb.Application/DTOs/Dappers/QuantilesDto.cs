namespace Airbnb.Application.DTOs.Dappers;

public record QuantilesDto
{
    public decimal? Q1 { get; init; }
    public decimal? Median { get; init; }
    public decimal? Q3 { get; init; }
}