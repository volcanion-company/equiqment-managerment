using EquipmentManagement.Application.Features.Maintenances.Commands.CompleteMaintenance;
using FluentAssertions;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Maintenances.Commands;

public class CompleteMaintenanceCommandValidatorTests
{
    private readonly CompleteMaintenanceCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CompleteMaintenanceCommand
        {
            MaintenanceRequestId = Guid.NewGuid(),
            TechnicianId = "tech123",
            Cost = 150.50m,
            CompletionNotes = "Fixed successfully",
            StillNeedsMaintenance = false
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ZeroCost_ShouldFail()
    {
        // Arrange
        var command = new CompleteMaintenanceCommand
        {
            MaintenanceRequestId = Guid.NewGuid(),
            TechnicianId = "tech123",
            Cost = 0
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Cost));
    }

    [Fact]
    public void Validate_NegativeCost_ShouldFail()
    {
        // Arrange
        var command = new CompleteMaintenanceCommand
        {
            MaintenanceRequestId = Guid.NewGuid(),
            TechnicianId = "tech123",
            Cost = -10
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Cost));
    }
}
