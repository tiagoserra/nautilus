using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure.Data.Contexts;

public class SqlServerContext : DbContext
{
    private readonly IDomainEventHandler _domainEventService;
    
    public SqlServerContext(DbContextOptions<SqlServerContext> options, IDomainEventHandler domainEventService)
        : base(options)
    {
        _domainEventService = domainEventService;
    }
    
    public SqlServerContext(DbContextOptions<SqlServerContext> option) : base(option) { }

    public SqlServerContext()
    {
    }

    #region DbSets

 	//%#DbSet#%
    
    #endregion
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

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

                    if (!entry.Entity.DomainValidation.IsValid())
                        throw new Exception(entry.Entity.DomainValidation.Errors.FirstOrDefault()?.Code);
                    
                    entry.Entity.SetCreation(string.Empty);

                    break;

                case EntityState.Modified:
                    
                    if (!entry.Entity.DomainValidation.IsValid())
                        throw new Exception(entry.Entity.DomainValidation.Errors.FirstOrDefault()?.Code);
                    
                    entry.Entity.SetModification(string.Empty);                    

                    break;
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