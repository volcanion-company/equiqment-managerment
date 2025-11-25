using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EquipmentManagement.UnitTests.Domain.Entities;

public class EquipmentTests
{
    [Fact]
    public void Equipment_ShouldBeCreated_WithValidProperties()
    {
        // Arrange
        var id = Guid.NewGuid();
        var code = "EQ001";
        var name = "Test Equipment";
        var type = "Computer";
        var price = 1000m;
        var status = EquipmentStatus.New;

        // Act
        var equipment = new Equipment
        {
            Id = id,
            Code = code,
            Name = name,
            Type = type,
            Price = price,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        // Assert
        equipment.Id.Should().Be(id);
        equipment.Code.Should().Be(code);
        equipment.Name.Should().Be(name);
        equipment.Type.Should().Be(type);
        equipment.Price.Should().Be(price);
        equipment.Status.Should().Be(status);
        equipment.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Equipment_ShouldHaveEmptyCollections_WhenCreated()
    {
        // Arrange & Act
        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Code = "EQ001",
            Name = "Test Equipment",
            Type = "Computer",
            Price = 1000,
            Status = EquipmentStatus.New
        };

        // Assert
        equipment.Assignments.Should().BeEmpty();
        equipment.MaintenanceRequests.Should().BeEmpty();
        equipment.AuditRecords.Should().BeEmpty();
        equipment.LiquidationRequests.Should().BeEmpty();
    }

    [Theory]
    [InlineData(EquipmentStatus.New)]
    [InlineData(EquipmentStatus.InUse)]
    [InlineData(EquipmentStatus.Broken)]
    [InlineData(EquipmentStatus.Repairing)]
    [InlineData(EquipmentStatus.Lost)]
    [InlineData(EquipmentStatus.Liquidated)]
    public void Equipment_ShouldAccept_AllValidStatuses(EquipmentStatus status)
    {
        // Arrange & Act
        var equipment = new Equipment
        {
            Id = Guid.NewGuid(),
            Code = "EQ001",
            Name = "Test Equipment",
            Type = "Computer",
            Price = 1000,
            Status = status
        };

        // Assert
        equipment.Status.Should().Be(status);
    }
}
