using Domain.Events;
using Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace Domain.Entites;

public abstract class Entity<T> : AbstractValidator<T>, IHasDomainEvent 
    where T : Entity<T>
{
    public long Id { get; }
    public DateTimeOffset CreatedOn { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTimeOffset ModifiedOn { get; private set; }
    public string ModifiedBy { get; private set; }
    public List<DomainEvent> Events { get; set; }
    public ValidationResult ValidationResult { get; protected set; }
    
    public abstract bool IsValid();

    public Entity()
    {
        ValidationResult = new ValidationResult();
    }
    
    public void SetCreation(string createdBy)
    {
        CreatedOn = DateTimeOffset.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetModification(string modifiedBy)
    {
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTimeOffset.UtcNow;
    }
}