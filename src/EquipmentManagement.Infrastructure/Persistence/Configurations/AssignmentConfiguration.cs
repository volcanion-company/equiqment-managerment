using EquipmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentManagement.Infrastructure.Persistence.Configurations;

public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
{
    public void Configure(EntityTypeBuilder<Assignment> builder)
    {
        builder.ToTable("Assignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AssignedToUserId)
            .HasMaxLength(100);

        builder.Property(a => a.AssignedToDepartment)
            .HasMaxLength(200);

        builder.Property(a => a.Notes)
            .HasMaxLength(500);

        builder.Property(a => a.AssignedBy)
            .HasMaxLength(100);

        builder.Property(a => a.Status)
            .IsRequired();

        builder.HasIndex(a => a.EquipmentId);
        builder.HasIndex(a => a.AssignedToUserId);
        builder.HasIndex(a => a.AssignedDate);
        builder.HasIndex(a => a.Status);
        builder.HasIndex(a => a.IsDeleted);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
