using EquipmentManagement.Application.Features.Assignments.Commands.UpdateAssignment;
using FluentAssertions;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Assignments.Commands;

public class UpdateAssignmentCommandValidatorTests
{
    private readonly UpdateAssignmentCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_ShouldPass()
    {
        // Arrange
        var command = new UpdateAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            AssignedDate = DateTime.UtcNow.AddDays(-1),
            Notes = "Updated notes",
            AssignedToUserId = "user123"
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
        var command = new UpdateAssignmentCommand
        {
            AssignmentId = Guid.Empty
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.AssignmentId));
    }

    [Fact]
    public void Validate_FutureAssignedDate_ShouldFail()
    {
        // Arrange
        var command = new UpdateAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            AssignedDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.AssignedDate));
    }

    [Fact]
    public void Validate_NotesTooLong_ShouldFail()
    {
        // Arrange
        var command = new UpdateAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            Notes = new string('A', 501)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Notes));
    }

    [Fact]
    public void Validate_UserIdTooLong_ShouldFail()
    {
        // Arrange
        var command = new UpdateAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            AssignedToUserId = new string('A', 101)
        };

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.AssignedToUserId));
    }
}
