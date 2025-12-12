using EquipmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EquipmentManagement.Infrastructure.Persistence.Configurations;

public class AuditRecordConfiguration : IEntityTypeConfiguration<AuditRecord>
{
    public void Configure(EntityTypeBuilder<AuditRecord> builder)
    {
        builder.ToTable("AuditRecords");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.CheckedByUserId)
            .HasMaxLength(100);

        builder.Property(a => a.Note)
            .HasMaxLength(1000);

        builder.Property(a => a.Location)
            .HasMaxLength(200);

        builder.Property(a => a.Result)
            .IsRequired();

        builder.HasIndex(a => a.EquipmentId);
        builder.HasIndex(a => a.CheckDate);
        builder.HasIndex(a => a.Result);
        builder.HasIndex(a => a.IsDeleted);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
