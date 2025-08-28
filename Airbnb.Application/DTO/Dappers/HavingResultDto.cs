namespace Airbnb.Application.DTO.Dappers;

public record HavingResultDto
{
    public string Location { get; set; }
    public int ApartmentCount { get; set; }
    public decimal AvgPrice { get; set; }
}