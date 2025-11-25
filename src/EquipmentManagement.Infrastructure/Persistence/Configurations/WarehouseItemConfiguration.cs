using EquipmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentManagement.Infrastructure.Persistence.Configurations;

public class WarehouseItemConfiguration : IEntityTypeConfiguration<WarehouseItem>
{
    public void Configure(EntityTypeBuilder<WarehouseItem> builder)
    {
        builder.ToTable("WarehouseItems");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.EquipmentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Quantity)
            .IsRequired();

        builder.Property(w => w.MinThreshold)
            .IsRequired();

        builder.Property(w => w.Notes)
            .HasMaxLength(500);

        builder.HasIndex(w => w.EquipmentType)
            .IsUnique();

        builder.HasIndex(w => w.IsDeleted);

        builder.HasQueryFilter(w => !w.IsDeleted);

        builder.HasMany(w => w.Transactions)
            .WithOne(t => t.WarehouseItem)
            .HasForeignKey(t => t.WarehouseItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
