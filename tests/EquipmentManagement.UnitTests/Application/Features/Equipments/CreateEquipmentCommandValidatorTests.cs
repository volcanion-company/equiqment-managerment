using EquipmentManagement.Application.Features.Equipments.Commands.CreateEquipment;
using FluentValidation.TestHelper;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Features.Equipments;

public class CreateEquipmentCommandValidatorTests
{
    private readonly CreateEquipmentCommandValidator _validator;

    public CreateEquipmentCommandValidatorTests()
    {
        _validator = new CreateEquipmentCommandValidator();
    }

    [Fact]
    public void Should_HaveError_When_CodeIsEmpty()
    {
        // Arrange
        var command = new CreateEquipmentCommand { Code = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Should_HaveError_When_NameIsEmpty()
    {
        // Arrange
        var command = new CreateEquipmentCommand { Name = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_HaveError_When_TypeIsEmpty()
    {
        // Arrange
        var command = new CreateEquipmentCommand { Category = string.Empty };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Should_HaveError_When_PriceIsNegative()
    {
        // Arrange
        var command = new CreateEquipmentCommand { PurchasePrice = -100 };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PurchasePrice);
    }

    [Fact]
    public void Should_HaveError_When_PurchaseDateIsInFuture()
    {
        // Arrange
        var command = new CreateEquipmentCommand { PurchaseDate = DateTime.UtcNow.AddDays(1) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PurchaseDate);
    }

    [Fact]
    public void Should_NotHaveError_When_CommandIsValid()
    {
        // Arrange
        var command = new CreateEquipmentCommand
        {
            Code = "EQ001",
            Name = "Test Equipment",
            Category = "Computer",
            PurchasePrice = 1000,
            PurchaseDate = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
