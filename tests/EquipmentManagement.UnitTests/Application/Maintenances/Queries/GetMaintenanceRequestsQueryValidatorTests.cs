using EquipmentManagement.Application.Features.Maintenances.Queries.GetMaintenanceRequests;
using FluentAssertions;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Maintenances.Queries;

public class GetMaintenanceRequestsQueryValidatorTests
{
    private readonly GetMaintenanceRequestsQueryValidator _validator = new();

    [Fact]
    public void Validate_ValidQuery_ShouldPass()
    {
        // Arrange
        var query = new GetMaintenanceRequestsQuery
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
        var query = new GetMaintenanceRequestsQuery
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

    [Theory]
    [InlineData(1)] // Pending
    [InlineData(2)] // InProgress
    [InlineData(3)] // Completed
    [InlineData(4)] // Cancelled
    public void Validate_ValidStatus_ShouldPass(int status)
    {
        // Arrange
        var query = new GetMaintenanceRequestsQuery
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
