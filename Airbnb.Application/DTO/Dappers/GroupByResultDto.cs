namespace Airbnb.Application.DTO.Dappers;

public record GroupByResultDto
{
    public string Location { get; set; }
    public int ApartmentCount { get; set; }
    public decimal AvgPrice { get; set; }
}