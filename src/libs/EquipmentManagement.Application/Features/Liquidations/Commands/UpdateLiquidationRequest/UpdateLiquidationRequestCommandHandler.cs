using EquipmentManagement.Domain.Repositories;
using MediatR;

namespace EquipmentManagement.Application.Features.Liquidations.Commands.UpdateLiquidationRequest;

public class UpdateLiquidationRequestCommandHandler(IUnitOfWork unitOfWork) 
    : IRequestHandler<UpdateLiquidationRequestCommand, Unit>
{
    public async Task<Unit> Handle(UpdateLiquidationRequestCommand request, CancellationToken cancellationToken)
    {
        // Get liquidation request
        var liquidationRequest = await unitOfWork.LiquidationRequests.GetByIdAsync(
            request.LiquidationRequestId, cancellationToken)
            ?? throw new KeyNotFoundException($"Liquidation request with ID {request.LiquidationRequestId} not found");

        // Validate: can only update pending requests
        if (liquidationRequest.IsApproved || liquidationRequest.ApprovedDate.HasValue)
            throw new InvalidOperationException("Cannot update liquidation request that has been approved or rejected");

        // Update fields if provided
        if (request.LiquidationValue.HasValue)
        {
            liquidationRequest.LiquidationValue = request.LiquidationValue.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Note))
        {
            liquidationRequest.Note = request.Note;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
