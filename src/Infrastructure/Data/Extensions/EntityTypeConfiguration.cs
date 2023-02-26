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

        builder.Property(p => p.Id)
            .HasColumnName("id");

        builder.Property(p => p.CreatedOn)
            .IsRequired()
            .HasColumnName("created_on")
            .HasColumnType("TIMESPAN");

        builder.Property(p => p.CreatedBy)
            .IsRequired()
            .HasColumnName("created_by")
            .HasMaxLength(255)
            .HasColumnType("VARCHAR(256)");

        builder.Property(p => p.ModifiedOn)
            .IsRequired(false)
            .HasColumnName("modified_on")
            .HasColumnType("TIMESPAN");

        builder.Property(p => p.ModifiedBy)
            .IsRequired(false)
            .HasColumnName("modified_by")
            .HasMaxLength(255)
            .HasColumnType("VARCHAR(256)");
    }
}