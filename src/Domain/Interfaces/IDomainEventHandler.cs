using Domain.Events;

namespace Domain.Interfaces;

public interface IDomainEventHandler
{
    Task Publish(DomainEvent moduleEvent);
}