using EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;
using EquipmentManagement.Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Features.Warehouses;

public class CreateWarehouseTransactionCommandValidatorTests
{
    private readonly CreateWarehouseTransactionCommandValidator _validator;

    public CreateWarehouseTransactionCommandValidatorTests()
    {
        _validator = new CreateWarehouseTransactionCommandValidator();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenValidCommand()
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.NewGuid(),
            Type = WarehouseTransactionType.Import,
            Quantity = 10,
            PerformedBy = "Admin",
            Reason = "Restocking"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenWarehouseItemIdIsEmpty()
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.Empty,
            Type = WarehouseTransactionType.Import,
            Quantity = 10,
            PerformedBy = "Admin"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WarehouseItemId)
            .WithErrorMessage("WarehouseItemId is required");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenQuantityIsZero()
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.NewGuid(),
            Type = WarehouseTransactionType.Import,
            Quantity = 0,
            PerformedBy = "Admin"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Quantity must be greater than 0");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenQuantityIsNegative()
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.NewGuid(),
            Type = WarehouseTransactionType.Import,
            Quantity = -5,
            PerformedBy = "Admin"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Quantity must be greater than 0");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPerformedByIsEmpty()
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.NewGuid(),
            Type = WarehouseTransactionType.Import,
            Quantity = 10,
            PerformedBy = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PerformedBy)
            .WithErrorMessage("PerformedBy is required");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPerformedByTooLong()
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.NewGuid(),
            Type = WarehouseTransactionType.Import,
            Quantity = 10,
            PerformedBy = new string('A', 201)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PerformedBy)
            .WithErrorMessage("PerformedBy must not exceed 200 characters");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenReasonTooLong()
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.NewGuid(),
            Type = WarehouseTransactionType.Import,
            Quantity = 10,
            PerformedBy = "Admin",
            Reason = new string('A', 501)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Reason)
            .WithErrorMessage("Reason must not exceed 500 characters");
    }

    [Theory]
    [InlineData(WarehouseTransactionType.Import)]
    [InlineData(WarehouseTransactionType.Export)]
    [InlineData(WarehouseTransactionType.Adjustment)]
    public void Validate_ShouldNotHaveError_WhenTypeIsValid(WarehouseTransactionType type)
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.NewGuid(),
            Type = type,
            Quantity = 10,
            PerformedBy = "Admin"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Type);
    }
}
