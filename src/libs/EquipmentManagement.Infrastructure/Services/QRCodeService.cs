using EquipmentManagement.Application.Common.Interfaces;
using QRCoder;

namespace EquipmentManagement.Infrastructure.Services;

public class QRCodeService : IQRCodeService
{
    public string GenerateQRCode(string data)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        
        var qrCodeImage = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeImage);
    }
}
