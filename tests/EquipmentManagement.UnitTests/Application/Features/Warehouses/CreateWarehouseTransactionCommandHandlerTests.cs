using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Application.Features.Warehouses.Commands.CreateTransaction;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Features.Warehouses;

public class CreateWarehouseTransactionCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWarehouseItemRepository> _warehouseItemRepositoryMock;
    private readonly Mock<IWarehouseTransactionRepository> _warehouseTransactionRepositoryMock;
    private readonly CreateWarehouseTransactionCommandHandler _handler;

    public CreateWarehouseTransactionCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _warehouseItemRepositoryMock = new Mock<IWarehouseItemRepository>();
        _warehouseTransactionRepositoryMock = new Mock<IWarehouseTransactionRepository>();

        _unitOfWorkMock
            .Setup(x => x.WarehouseItems)
            .Returns(_warehouseItemRepositoryMock.Object);

        _unitOfWorkMock
            .Setup(x => x.WarehouseTransactions)
            .Returns(_warehouseTransactionRepositoryMock.Object);

        _handler = new CreateWarehouseTransactionCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldImportItems_WhenTypeIsImport()
    {
        // Arrange
        var warehouseItem = new WarehouseItem
        {
            Id = Guid.NewGuid(),
            EquipmentType = "Laptop",
            Quantity = 10,
            MinThreshold = 5,
            IsDeleted = false
        };

        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = warehouseItem.Id,
            Type = WarehouseTransactionType.Import,
            Quantity = 20,
            PerformedBy = "Admin",
            Reason = "Restocking"
        };

        _warehouseItemRepositoryMock
            .Setup(x => x.GetByIdAsync(warehouseItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouseItem);

        _warehouseTransactionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<WarehouseTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WarehouseTransaction t, CancellationToken ct) => t);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        warehouseItem.Quantity.Should().Be(30); // 10 + 20

        _warehouseItemRepositoryMock.Verify(
            x => x.Update(It.Is<WarehouseItem>(item => item.Quantity == 30)),
            Times.Once);

        _warehouseTransactionRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<WarehouseTransaction>(t =>
                    t.WarehouseItemId == warehouseItem.Id &&
                    t.Type == WarehouseTransactionType.Import &&
                    t.Quantity == 20),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldExportItems_WhenTypeIsExportAndSufficientQuantity()
    {
        // Arrange
        var warehouseItem = new WarehouseItem
        {
            Id = Guid.NewGuid(),
            EquipmentType = "Laptop",
            Quantity = 30,
            MinThreshold = 5,
            IsDeleted = false
        };

        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = warehouseItem.Id,
            Type = WarehouseTransactionType.Export,
            Quantity = 10,
            PerformedBy = "Admin",
            Reason = "Assignment"
        };

        _warehouseItemRepositoryMock
            .Setup(x => x.GetByIdAsync(warehouseItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouseItem);

        _warehouseTransactionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<WarehouseTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WarehouseTransaction t, CancellationToken ct) => t);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        warehouseItem.Quantity.Should().Be(20); // 30 - 10

        _warehouseItemRepositoryMock.Verify(
            x => x.Update(It.Is<WarehouseItem>(item => item.Quantity == 20)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenExportingMoreThanAvailable()
    {
        // Arrange
        var warehouseItem = new WarehouseItem
        {
            Id = Guid.NewGuid(),
            EquipmentType = "Laptop",
            Quantity = 5,
            MinThreshold = 2,
            IsDeleted = false
        };

        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = warehouseItem.Id,
            Type = WarehouseTransactionType.Export,
            Quantity = 10,
            PerformedBy = "Admin"
        };

        _warehouseItemRepositoryMock
            .Setup(x => x.GetByIdAsync(warehouseItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouseItem);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Insufficient quantity in warehouse*");

        _warehouseTransactionRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<WarehouseTransaction>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenWarehouseItemDoesNotExist()
    {
        // Arrange
        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = Guid.NewGuid(),
            Type = WarehouseTransactionType.Import,
            Quantity = 10,
            PerformedBy = "Admin"
        };

        _warehouseItemRepositoryMock
            .Setup(x => x.GetByIdAsync(command.WarehouseItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WarehouseItem?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldAdjustQuantity_WhenTypeIsAdjustment()
    {
        // Arrange
        var warehouseItem = new WarehouseItem
        {
            Id = Guid.NewGuid(),
            EquipmentType = "Laptop",
            Quantity = 10,
            MinThreshold = 5,
            IsDeleted = false
        };

        var command = new CreateWarehouseTransactionCommand
        {
            WarehouseItemId = warehouseItem.Id,
            Type = WarehouseTransactionType.Adjustment,
            Quantity = 15, // New total
            PerformedBy = "Admin",
            Reason = "Inventory correction"
        };

        _warehouseItemRepositoryMock
            .Setup(x => x.GetByIdAsync(warehouseItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouseItem);

        _warehouseTransactionRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<WarehouseTransaction>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WarehouseTransaction t, CancellationToken ct) => t);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        warehouseItem.Quantity.Should().Be(15);

        _warehouseTransactionRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<WarehouseTransaction>(t =>
                    t.Type == WarehouseTransactionType.Adjustment &&
                    t.Quantity == 5), // Delta: 15 - 10
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
