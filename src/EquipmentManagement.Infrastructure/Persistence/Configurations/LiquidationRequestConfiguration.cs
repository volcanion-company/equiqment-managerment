using EquipmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentManagement.Infrastructure.Persistence.Configurations;

public class LiquidationRequestConfiguration : IEntityTypeConfiguration<LiquidationRequest>
{
    public void Configure(EntityTypeBuilder<LiquidationRequest> builder)
    {
        builder.ToTable("LiquidationRequests");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.ApprovedBy)
            .HasMaxLength(100);

        builder.Property(l => l.LiquidationValue)
            .HasColumnType("decimal(18,2)");

        builder.Property(l => l.Note)
            .HasMaxLength(1000);

        builder.Property(l => l.IsApproved)
            .IsRequired();

        builder.HasIndex(l => l.EquipmentId);
        builder.HasIndex(l => l.RequestDate);
        builder.HasIndex(l => l.IsApproved);
        builder.HasIndex(l => l.IsDeleted);

        builder.HasQueryFilter(l => !l.IsDeleted);
    }
}
