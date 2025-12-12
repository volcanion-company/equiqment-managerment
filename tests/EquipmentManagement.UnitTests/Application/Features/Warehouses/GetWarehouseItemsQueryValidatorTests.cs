using EquipmentManagement.Application.Features.Warehouses.Queries.GetWarehouseItems;
using FluentValidation.TestHelper;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Features.Warehouses;

public class GetWarehouseItemsQueryValidatorTests
{
    private readonly GetWarehouseItemsQueryValidator _validator;

    public GetWarehouseItemsQueryValidatorTests()
    {
        _validator = new GetWarehouseItemsQueryValidator();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenValidQuery()
    {
        // Arrange
        var query = new GetWarehouseItemsQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageNumberIsZero()
    {
        // Arrange
        var query = new GetWarehouseItemsQuery
        {
            PageNumber = 0,
            PageSize = 10
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber)
            .WithErrorMessage("PageNumber must be at least 1");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageSizeIsZero()
    {
        // Arrange
        var query = new GetWarehouseItemsQuery
        {
            PageNumber = 1,
            PageSize = 0
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be greater than 0");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPageSizeExceeds100()
    {
        // Arrange
        var query = new GetWarehouseItemsQuery
        {
            PageNumber = 1,
            PageSize = 101
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must not exceed 100");
    }
}
