using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Commands.CreateAuditRecord;

public class CreateAuditRecordCommandHandler : IRequestHandler<CreateAuditRecordCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateAuditRecordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateAuditRecordCommand request, CancellationToken cancellationToken)
    {
        var equipment = await _unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.EquipmentId);
        }

        var auditRecord = request.Adapt<AuditRecord>();
        auditRecord.Id = Guid.NewGuid();
        auditRecord.CreatedAt = DateTime.UtcNow;
        auditRecord.LastSyncDate = DateTime.UtcNow;
        auditRecord.IsDeleted = false;

        await _unitOfWork.AuditRecords.AddAsync(auditRecord, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return auditRecord.Id;
    }
}
