using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Audits.Commands.UpdateAuditRecord;

public class UpdateAuditRecordCommandHandler : IRequestHandler<UpdateAuditRecordCommand, Unit>
{
    private readonly IAuditRecordRepository _auditRecordRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAuditRecordCommandHandler(
        IAuditRecordRepository auditRecordRepository,
        IUnitOfWork unitOfWork)
    {
        _auditRecordRepository = auditRecordRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateAuditRecordCommand request, CancellationToken cancellationToken)
    {
        var auditRecord = await _auditRecordRepository.GetByIdAsync(request.AuditRecordId, cancellationToken)
            ?? throw new KeyNotFoundException($"Audit record with ID {request.AuditRecordId} not found");

        // Update audit record properties
        auditRecord.Result = request.Result;
        auditRecord.Note = request.Note;
        auditRecord.Location = request.Location;
        auditRecord.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
