namespace Airbnb.Domain.ValueObject;

public record DateRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public DateRange(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("Error with time!");
        
        Start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        End = DateTime.SpecifyKind(end, DateTimeKind.Utc);
    }

    public bool Intersect(DateRange date)
    {
        return Start < date.End && End > date.Start;
    }
}