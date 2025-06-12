namespace Airbnb.Domain.ValueObject;

public class DateRange
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public DateRange(DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("Error with time!");
        
        Start = start;
        End = end;
    }

    public bool Intersect(DateRange date)
    {
        return Start < date.End && End > date.Start;
    }
}