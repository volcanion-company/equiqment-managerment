namespace EquipmentManagement.Application.Common.Interfaces;

public interface IQRCodeService
{
    string GenerateQRCode(string data);
}
