using Domain.Events;
using Domain.Interfaces;

namespace Domain.Entities;

public abstract class Entity: IHasDomainEvent
{
    public long Id { get; }
    public DateTime CreatedOn { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime? ModifiedOn { get; private set; }
    public string ModifiedBy { get; private set; }
    public List<DomainEvent> Events { get; set; }
    
    public void SetCreation(string createdBy)
    {
        CreatedOn = DateTime.Now;
        CreatedBy = createdBy;
    }

    public void SetModification(string modifiedBy)
    {
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.Now;
    }
}