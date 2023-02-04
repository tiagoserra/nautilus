using Domain.Entities;

namespace Domain.Events;

public class AuditEvent<TEntity> : DomainEvent where TEntity : Entity
{
    public TEntity Entity { get; }
    
    public string Action { get; }

    public AuditEvent(TEntity entity, string action)
    {
        Entity = entity;
        Action = action;
    }
}