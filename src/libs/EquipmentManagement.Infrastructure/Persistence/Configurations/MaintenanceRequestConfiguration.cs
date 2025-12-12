using EquipmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentManagement.Infrastructure.Persistence.Configurations;

public class MaintenanceRequestConfiguration : IEntityTypeConfiguration<MaintenanceRequest>
{
    public void Configure(EntityTypeBuilder<MaintenanceRequest> builder)
    {
        builder.ToTable("MaintenanceRequests");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.RequesterId)
            .HasMaxLength(100);

        builder.Property(m => m.TechnicianId)
            .HasMaxLength(100);

        builder.Property(m => m.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        builder.Property(m => m.Cost)
            .HasColumnType("decimal(18,2)");

        builder.Property(m => m.Status)
            .IsRequired();

        builder.HasIndex(m => m.EquipmentId);
        builder.HasIndex(m => m.TechnicianId);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.RequestDate);
        builder.HasIndex(m => m.IsDeleted);

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
