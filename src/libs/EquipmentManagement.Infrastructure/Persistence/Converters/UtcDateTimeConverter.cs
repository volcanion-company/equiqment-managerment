using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EquipmentManagement.Infrastructure.Persistence;

/// <summary>
/// Converts DateTime values to UTC when saving to database and ensures Kind is set to UTC when reading.
/// </summary>
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter()
        : base(
            // Convert to UTC when writing to database
            v => v.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(v, DateTimeKind.Utc)
                : v.ToUniversalTime(),
            // Ensure UTC kind when reading from database
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}

/// <summary>
/// Converts nullable DateTime values to UTC when saving to database and ensures Kind is set to UTC when reading.
/// </summary>
public class UtcNullableDateTimeConverter : ValueConverter<DateTime?, DateTime?>
{
    public UtcNullableDateTimeConverter()
        : base(
            // Convert to UTC when writing to database
            v => v.HasValue
                ? (v.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                    : v.Value.ToUniversalTime())
                : v,
            // Ensure UTC kind when reading from database
            v => v.HasValue
                ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                : v)
    {
    }
}
