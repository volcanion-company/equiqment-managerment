using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Application.Features.Assignments.Commands.ReturnAssignment;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Assignments.Commands;

public class ReturnAssignmentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAssignmentRepository> _assignmentRepoMock;
    private readonly Mock<IEquipmentRepository> _equipmentRepoMock;
    private readonly Mock<IWarehouseItemRepository> _warehouseItemRepoMock;
    private readonly Mock<IWarehouseTransactionRepository> _warehouseTransactionRepoMock;
    private readonly ReturnAssignmentCommandHandler _handler;

    public ReturnAssignmentCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _assignmentRepoMock = new Mock<IAssignmentRepository>();
        _equipmentRepoMock = new Mock<IEquipmentRepository>();
        _warehouseItemRepoMock = new Mock<IWarehouseItemRepository>();
        _warehouseTransactionRepoMock = new Mock<IWarehouseTransactionRepository>();

        _unitOfWorkMock.Setup(u => u.Assignments).Returns(_assignmentRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Equipments).Returns(_equipmentRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.WarehouseItems).Returns(_warehouseItemRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.WarehouseTransactions).Returns(_warehouseTransactionRepoMock.Object);

        _handler = new ReturnAssignmentCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidAssignment_ShouldReturnSuccessfully()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();
        var warehouseItemId = Guid.NewGuid();

        var assignment = new Assignment
        {
            Id = assignmentId,
            EquipmentId = equipmentId,
            Status = AssignmentStatus.Assigned,
            AssignedToUserId = "user123",
            Notes = "Original notes"
        };

        var equipment = new Equipment
        {
            Id = equipmentId,
            Type = "Laptop",
            Status = EquipmentStatus.InUse
        };

        var warehouseItem = new WarehouseItem
        {
            Id = warehouseItemId,
            EquipmentType = "Laptop",
            Quantity = 5
        };

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignmentId, default))
            .ReturnsAsync(assignment);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(equipmentId, default))
            .ReturnsAsync(equipment);
        _warehouseItemRepoMock.Setup(r => r.GetByEquipmentTypeAsync("Laptop", default))
            .ReturnsAsync(warehouseItem);

        var command = new ReturnAssignmentCommand
        {
            AssignmentId = assignmentId,
            ReturnedBy = "Admin",
            ReturnNotes = "Returned in good condition",
            NeedsMaintenance = false
        };

        // Act
        await _handler.Handle(command, default);

        // Assert
        assignment.Status.Should().Be(AssignmentStatus.Returned);
        assignment.ReturnDate.Should().NotBeNull();
        equipment.Status.Should().Be(EquipmentStatus.New);
        warehouseItem.Quantity.Should().Be(6);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_AssignmentNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Assignment?)null);

        var command = new ReturnAssignmentCommand
        {
            AssignmentId = Guid.NewGuid(),
            ReturnedBy = "Admin"
        };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_AlreadyReturnedAssignment_ShouldThrowValidationException()
    {
        // Arrange
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Status = AssignmentStatus.Returned
        };

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync(assignment);

        var command = new ReturnAssignmentCommand
        {
            AssignmentId = assignment.Id,
            ReturnedBy = "Admin"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, default));
    }

    [Fact]
    public async Task Handle_NeedsMaintenance_ShouldSetEquipmentStatusToRepairing()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var equipmentId = Guid.NewGuid();

        var assignment = new Assignment
        {
            Id = assignmentId,
            EquipmentId = equipmentId,
            Status = AssignmentStatus.Assigned
        };

        var equipment = new Equipment
        {
            Id = equipmentId,
            Type = "Laptop",
            Status = EquipmentStatus.InUse
        };

        var warehouseItem = new WarehouseItem
        {
            EquipmentType = "Laptop",
            Quantity = 1
        };

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(assignmentId, default))
            .ReturnsAsync(assignment);
        _equipmentRepoMock.Setup(r => r.GetByIdAsync(equipmentId, default))
            .ReturnsAsync(equipment);
        _warehouseItemRepoMock.Setup(r => r.GetByEquipmentTypeAsync("Laptop", default))
            .ReturnsAsync(warehouseItem);

        var command = new ReturnAssignmentCommand
        {
            AssignmentId = assignmentId,
            ReturnedBy = "Admin",
            NeedsMaintenance = true
        };

        // Act
        await _handler.Handle(command, default);

        // Assert
        equipment.Status.Should().Be(EquipmentStatus.Repairing);
    }
}
