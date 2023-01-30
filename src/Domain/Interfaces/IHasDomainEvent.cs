using Domain.Events;

namespace Domain.Interfaces;

public interface IHasDomainEvent
{
    public List<DomainEvent> Events { get; set; }
}