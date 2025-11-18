namespace Airbnb.Application.DTO.Pagination;

public record PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; }
    public int TotalCount { get; set; }
}