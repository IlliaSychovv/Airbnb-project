namespace Airbnb.Application.DTOs;

public class PagedResponse<Apartment>
{
    public List<Apartment> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => CurrentPage < TotalPages;
}