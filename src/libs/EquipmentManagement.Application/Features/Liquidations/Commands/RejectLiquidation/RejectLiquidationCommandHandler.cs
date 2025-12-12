using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.RejectLiquidation;

public class RejectLiquidationCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<RejectLiquidationCommand, Unit>
{
    public async Task<Unit> Handle(RejectLiquidationCommand request, CancellationToken cancellationToken)
    {
        // Get liquidation request
        var liquidationRequest = await unitOfWork.LiquidationRequests.GetByIdAsync(
            request.LiquidationRequestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Liquidation request with ID {request.LiquidationRequestId} not found");

        // Validate: must be pending (not already approved or rejected)
        if (liquidationRequest.IsApproved)
            throw new InvalidOperationException("Cannot reject an already approved liquidation request");

        // Mark as rejected by setting IsApproved to false explicitly
        // Note: In the current schema, IsApproved is bool, not bool?
        // We'll use ApprovedBy and ApprovedDate being null to indicate pending
        // And IsApproved = false with rejection note to indicate rejection
        liquidationRequest.IsApproved = false;
        liquidationRequest.ApprovedBy = request.RejectedBy;
        liquidationRequest.ApprovedDate = DateTime.UtcNow;

        // Append rejection reason to notes
        liquidationRequest.Note = string.IsNullOrWhiteSpace(liquidationRequest.Note)
            ? $"[REJECTED] {request.RejectionReason}"
            : $"{liquidationRequest.Note}\n[REJECTED] {request.RejectionReason}";

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
