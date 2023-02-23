using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Contexts;

public class SqlContext : DbContext
{
    private readonly IDomainEventHandler _domainEventService;

    public SqlContext(DbContextOptions<SqlContext> options, IDomainEventHandler domainEventService)
        : base(options)
        => _domainEventService = domainEventService;

    public SqlContext(DbContextOptions<SqlContext> option) : base(option)
    {
    }

    public SqlContext()
    {
    }

    #region DbSets

    //%#DbSet#%

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var dateTimeConverter =
            new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    property.SetValueConverter(dateTimeConverter);
            }
        }

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    
                    entry.Entity.SetCreation(string.Empty);
                    entry.Entity.Events.Add(new AuditEvent<Entity>(entry.Entity, "insert"));
                    
                    break;

                case EntityState.Modified:

                    entry.Entity.SetModification(string.Empty);
                    entry.Entity.Events.Add(new AuditEvent<Entity>(entry.Entity, "Update"));

                    break;
                
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    
                    entry.Entity.SetModification(string.Empty);
                    entry.Entity.Events.Add(new AuditEvent<Entity>(entry.Entity, "Delete"));

                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents();

        return result;
    }

    private async Task DispatchEvents()
    {
        while (true)
        {
            var domainEventEntity = ChangeTracker
                .Entries<IHasDomainEvent>()
                .Select(x => x.Entity.Events)
                .SelectMany(x => x)
                .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

            if (domainEventEntity is null) break;

            domainEventEntity.IsPublished = true;
            await _domainEventService.Publish(domainEventEntity);
        }
    }
}