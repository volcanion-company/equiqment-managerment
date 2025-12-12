using EquipmentManagement.Application.Common.Interfaces;
using EquipmentManagement.Application.Features.Equipments.Commands.CreateEquipment;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Enums;
using EquipmentManagement.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace EquipmentManagement.UnitTests.Application.Features.Equipments;

public class CreateEquipmentCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IQRCodeService> _qrCodeServiceMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly CreateEquipmentCommandHandler _handler;

    public CreateEquipmentCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _qrCodeServiceMock = new Mock<IQRCodeService>();
        _cacheServiceMock = new Mock<ICacheService>();

        _handler = new CreateEquipmentCommandHandler(
            _unitOfWorkMock.Object,
            _qrCodeServiceMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateEquipment_WhenValidCommand()
    {
        // Arrange
        var command = new CreateEquipmentCommand
        {
            Code = "EQ001",
            Name = "Test Equipment",
            Type = "Computer",
            Price = 1000,
            PurchaseDate = DateTime.UtcNow,
            Status = EquipmentStatus.New
        };

        var qrCode = "base64-qr-code";
        _qrCodeServiceMock
            .Setup(x => x.GenerateQRCode(command.Code))
            .Returns(qrCode);

        var equipmentRepositoryMock = new Mock<IEquipmentRepository>();
        _unitOfWorkMock
            .Setup(x => x.Equipments)
            .Returns(equipmentRepositoryMock.Object);

        equipmentRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Equipment e, CancellationToken ct) => e);

        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeEmpty();
        
        equipmentRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<Equipment>(e => 
                    e.Code == command.Code && 
                    e.Name == command.Name &&
                    e.QRCodeBase64 == qrCode),
                It.IsAny<CancellationToken>()), 
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), 
            Times.Once);

        _cacheServiceMock.Verify(
            x => x.RemoveByPrefixAsync("equipments_", It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldGenerateQRCode_WhenCreatingEquipment()
    {
        // Arrange
        var command = new CreateEquipmentCommand
        {
            Code = "EQ002",
            Name = "Test Equipment 2",
            Type = "Laptop",
            Price = 2000,
            PurchaseDate = DateTime.UtcNow,
            Status = EquipmentStatus.New
        };

        var equipmentRepositoryMock = new Mock<IEquipmentRepository>();
        _unitOfWorkMock.Setup(x => x.Equipments).Returns(equipmentRepositoryMock.Object);

        equipmentRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Equipment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Equipment e, CancellationToken ct) => e);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _qrCodeServiceMock.Verify(
            x => x.GenerateQRCode(command.Code), 
            Times.Once);
    }
}
