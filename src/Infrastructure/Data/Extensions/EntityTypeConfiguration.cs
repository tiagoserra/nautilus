using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Extensions;

public class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Ignore(e => e.Events);

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("Id");

        builder.Property(p => p.CreatedOn).IsRequired().HasColumnName("CreatedOn").HasColumnType("DATETIME");

        builder.Property(p => p.CreatedBy).IsRequired().HasColumnName("CreatedBy").HasMaxLength(255).HasColumnType("VARCHAR(255)");

        builder.Property(p => p.ModifiedOn).IsRequired(false).HasColumnName("ModifiedOn").HasColumnType("DATETIME");

        builder.Property(p => p.ModifiedBy).IsRequired(false).HasColumnName("ModifiedBy").HasMaxLength(255).HasColumnType("VARCHAR(255)");
    }
}