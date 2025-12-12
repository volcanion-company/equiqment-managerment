using EquipmentManagement.Application.Features.Assignments.Commands.ReturnAssignment;
using FluentAssertions;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Assignments.Commands;

public class ReturnAssignmentCommandValidatorTests
{
    private readonly ReturnAssignmentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new ReturnAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            ReturnedBy = "Admin User",
            ReturnNotes = "Equipment in good condition",
            NeedsMaintenance = false
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyAssignmentId_ShouldFail()
    {
        // Arrange
        var command = new ReturnAssignmentCommand
        {
            AssignmentId = Guid.Empty,
            ReturnedBy = "Admin User"
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.AssignmentId));
    }

    [Fact]
    public void Validate_MissingReturnedBy_ShouldFail()
    {
        // Arrange
        var command = new ReturnAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            ReturnedBy = null
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.ReturnedBy));
    }

    [Fact]
    public void Validate_ReturnedByTooLong_ShouldFail()
    {
        // Arrange
        var command = new ReturnAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            ReturnedBy = new string('A', 101)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.ReturnedBy));
    }

    [Fact]
    public void Validate_ReturnNotesTooLong_ShouldFail()
    {
        // Arrange
        var command = new ReturnAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            ReturnedBy = "Admin",
            ReturnNotes = new string('A', 501)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.ReturnNotes));
    }
}
