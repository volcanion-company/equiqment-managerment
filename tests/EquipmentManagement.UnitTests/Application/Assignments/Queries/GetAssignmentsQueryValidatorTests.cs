using EquipmentManagement.Application.Features.Assignments.Queries.GetAssignments;
using FluentAssertions;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Assignments.Queries;

public class GetAssignmentsQueryValidatorTests
{
    private readonly GetAssignmentsQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ShouldPass()
    {
        // Arrange
        var query = new GetAssignmentsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            Status = 1
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_PageNumberZero_ShouldFail()
    {
        // Arrange
        var query = new GetAssignmentsQuery
        {
            PageNumber = 0,
            PageSize = 10
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.PageNumber));
    }

    [Fact]
    public void Validate_PageSizeZero_ShouldFail()
    {
        // Arrange
        var query = new GetAssignmentsQuery
        {
            PageNumber = 1,
            PageSize = 0
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.PageSize));
    }

    [Fact]
    public void Validate_PageSizeTooLarge_ShouldFail()
    {
        // Arrange
        var query = new GetAssignmentsQuery
        {
            PageNumber = 1,
            PageSize = 101
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.PageSize));
    }

    [Fact]
    public void Validate_InvalidStatus_ShouldFail()
    {
        // Arrange
        var query = new GetAssignmentsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            Status = 99
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.Status));
    }

    [Theory]
    [InlineData(1)] // Assigned
    [InlineData(2)] // Returned
    [InlineData(3)] // Lost
    public void Validate_ValidStatus_ShouldPass(int status)
    {
        // Arrange
        var query = new GetAssignmentsQuery
        {
            PageNumber = 1,
            PageSize = 10,
            Status = status
        };

        // Act
        var result = _validator.Validate(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
