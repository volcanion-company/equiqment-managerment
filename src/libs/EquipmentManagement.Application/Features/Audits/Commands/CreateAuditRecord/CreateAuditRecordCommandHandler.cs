using EquipmentManagement.Application.Common.Exceptions;
using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using Mapster;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Commands.CreateAuditRecord;

public class CreateAuditRecordCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateAuditRecordCommand, Guid>
{
    public async Task<Guid> Handle(CreateAuditRecordCommand request, CancellationToken cancellationToken)
    {
        var equipment = await unitOfWork.Equipments.GetByIdAsync(request.EquipmentId, cancellationToken);
        
        if (equipment == null || equipment.IsDeleted)
        {
            throw new NotFoundException(nameof(Equipment), request.EquipmentId);
        }

        var auditRecord = request.Adapt<AuditRecord>();
        auditRecord.Id = Guid.NewGuid();
        auditRecord.CreatedAt = DateTime.UtcNow;
        auditRecord.LastSyncDate = DateTime.UtcNow;
        auditRecord.IsDeleted = false;

        await unitOfWork.AuditRecords.AddAsync(auditRecord, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return auditRecord.Id;
    }
}
