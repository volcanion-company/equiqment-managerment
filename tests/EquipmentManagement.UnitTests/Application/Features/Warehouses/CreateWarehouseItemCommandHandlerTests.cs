using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Application.Features.Warehouses.Commands.CreateWarehouseItem;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Features.Warehouses;

public class CreateWarehouseItemCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWarehouseItemRepository> _warehouseItemRepositoryMock;
    private readonly CreateWarehouseItemCommandHandler _handler;

    public CreateWarehouseItemCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _warehouseItemRepositoryMock = new Mock<IWarehouseItemRepository>();

        _unitOfWorkMock
            .Setup(x => x.WarehouseItems)
            .Returns(_warehouseItemRepositoryMock.Object);

        _handler = new CreateWarehouseItemCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateWarehouseItem_WhenValidCommand()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = "Laptop",
            Quantity = 10,
            MinThreshold = 5,
            Notes = "Test notes"
        };

        _warehouseItemRepositoryMock
            .Setup(x => x.GetByEquipmentTypeAsync(command.EquipmentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WarehouseItem?)null);

        _warehouseItemRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<WarehouseItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WarehouseItem item, CancellationToken ct) => item);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _warehouseItemRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<WarehouseItem>(item =>
                    item.EquipmentType == command.EquipmentType &&
                    item.Quantity == command.Quantity &&
                    item.MinThreshold == command.MinThreshold &&
                    item.Notes == command.Notes &&
                    !item.IsDeleted),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenEquipmentTypeAlreadyExists()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = "Laptop",
            Quantity = 10,
            MinThreshold = 5
        };

        var existingItem = new WarehouseItem
        {
            Id = Guid.NewGuid(),
            EquipmentType = "Laptop",
            Quantity = 5,
            MinThreshold = 2,
            IsDeleted = false
        };

        _warehouseItemRepositoryMock
            .Setup(x => x.GetByEquipmentTypeAsync(command.EquipmentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Equipment type already exists in warehouse*");

        _warehouseItemRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<WarehouseItem>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateWarehouseItem_WhenEquipmentTypeExistsButIsDeleted()
    {
        // Arrange
        var command = new CreateWarehouseItemCommand
        {
            EquipmentType = "Laptop",
            Quantity = 10,
            MinThreshold = 5
        };

        var deletedItem = new WarehouseItem
        {
            Id = Guid.NewGuid(),
            EquipmentType = "Laptop",
            Quantity = 0,
            MinThreshold = 2,
            IsDeleted = true
        };

        _warehouseItemRepositoryMock
            .Setup(x => x.GetByEquipmentTypeAsync(command.EquipmentType, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedItem);

        _warehouseItemRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<WarehouseItem>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WarehouseItem item, CancellationToken ct) => item);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();

        _warehouseItemRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<WarehouseItem>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
