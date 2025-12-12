using EquipmentManagement.Application.Features.Warehouses.Commands.CreateWarehouseItem;
using FluentValidation.TestHelper;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Features.Warehouses;

public class CreateWarehouseItemCommandValidatorTests
{
    private readonly CreateWarehouseItemCommandValidator _validator;

    public CreateWarehouseItemCommandValidatorTests()
    {
        _validator = new CreateWarehouseItemCommandValidator();
    }

    [Fact]
    public void Validate_ShouldNotHaveError_WhenValidCommand()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = "Laptop",
            Quantity = 10,
            MinThreshold = 5,
            Notes = "Test notes"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEquipmentTypeIsEmpty()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = "",
            Quantity = 10,
            MinThreshold = 5
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EquipmentType)
            .WithErrorMessage("EquipmentType is required");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEquipmentTypeTooLong()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = new string('A', 201),
            Quantity = 10,
            MinThreshold = 5
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EquipmentType)
            .WithErrorMessage("EquipmentType must not exceed 200 characters");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenQuantityIsNegative()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = "Laptop",
            Quantity = -1,
            MinThreshold = 5
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity)
            .WithErrorMessage("Quantity must be greater than or equal to 0");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenMinThresholdIsNegative()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = "Laptop",
            Quantity = 10,
            MinThreshold = -1
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MinThreshold)
            .WithErrorMessage("MinThreshold must be greater than or equal to 0");
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenNotesTooLong()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = "Laptop",
            Quantity = 10,
            MinThreshold = 5,
            Notes = new string('A', 501)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Notes)
            .WithErrorMessage("Notes must not exceed 500 characters");
    }
}
