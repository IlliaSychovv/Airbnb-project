namespace Airbnb.Application.DTO.Dappers;

public record AggregateStatsDto
{
    public int Count { get; set; }
    public decimal AvgPrice { get; set; }
    public decimal SumPrice { get; set; }
    public decimal MaxPrice { get; set; }
}