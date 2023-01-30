namespace Domain.Events;

public class DomainEvent
{
    public bool IsPublished { get; set; }
    public DateTimeOffset DataOccurred { get; }

    public DomainEvent()
    {
        DataOccurred = DateTime.UtcNow;
    }

}