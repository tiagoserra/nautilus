using Domain.Events;
using Domain.Interfaces;
using MediatR;

namespace Infrastructure.EventHandlers;

public class DomainEventHandler : IDomainEventHandler
{
    readonly IPublisher _mediator;

    public DomainEventHandler(IPublisher mediator) => _mediator = mediator;

    public async Task Publish(DomainEvent domainEvent)
        => await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));

    private INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
        => (INotification)Activator.CreateInstance(typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent);
}