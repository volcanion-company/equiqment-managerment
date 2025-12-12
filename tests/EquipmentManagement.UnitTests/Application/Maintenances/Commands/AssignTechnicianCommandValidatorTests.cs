using EquipmentManagement.Application.Features.Maintenances.Commands.AssignTechnician;
using FluentAssertions;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Maintenances.Commands;

public class AssignTechnicianCommandValidatorTests
{
    private readonly AssignTechnicianCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new AssignTechnicianCommand
        {
            MaintenanceRequestId = Guid.NewGuid(),
            TechnicianId = "tech123",
            AssignmentNotes = "Assigned to senior technician"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyTechnicianId_ShouldFail()
    {
        // Arrange
        var command = new AssignTechnicianCommand
        {
            MaintenanceRequestId = Guid.NewGuid(),
            TechnicianId = ""
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.TechnicianId));
    }
}
