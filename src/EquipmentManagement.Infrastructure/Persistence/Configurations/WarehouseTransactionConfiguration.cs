using EquipmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentManagement.Infrastructure.Persistence.Configurations;

public class WarehouseTransactionConfiguration : IEntityTypeConfiguration<WarehouseTransaction>
{
    public void Configure(EntityTypeBuilder<WarehouseTransaction> builder)
    {
        builder.ToTable("WarehouseTransactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type)
            .IsRequired();

        builder.Property(t => t.Quantity)
            .IsRequired();

        builder.Property(t => t.Reason)
            .HasMaxLength(500);

        builder.Property(t => t.PerformedBy)
            .HasMaxLength(100);

        builder.HasIndex(t => t.WarehouseItemId);
        builder.HasIndex(t => t.TransactionDate);
        builder.HasIndex(t => t.IsDeleted);

        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
