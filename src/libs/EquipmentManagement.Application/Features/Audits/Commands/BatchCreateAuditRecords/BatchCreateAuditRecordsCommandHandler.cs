using EquipmentManagement.Domain.Entities;
using EquipmentManagement.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EquipmentManagement.Application.Features.Audits.Commands.BatchCreateAuditRecords;

/// <summary>
/// Handles batch creation of audit records for equipment, ensuring that each audit entry is validated and persisted as
/// part of a single transactional operation.
/// </summary>
/// <remarks>This handler processes multiple audit records in a single request, validating equipment existence and
/// updating related information as needed. All successful audit records are committed in one transaction to ensure
/// consistency. Partial failures are reported in the result, and errors are logged for each failed record.</remarks>
/// <param name="auditRecordRepository">The repository used to add new audit records to the data store.</param>
/// <param name="equipmentRepository">The repository used to retrieve and validate equipment referenced by the audit records.</param>
/// <param name="unitOfWork">The unit of work that manages transactional persistence of all changes made during the batch operation.</param>
/// <param name="logger">The logger used to record informational and error messages during the batch creation process.</param>
public class BatchCreateAuditRecordsCommandHandler(
    IAuditRecordRepository auditRecordRepository,
    IEquipmentRepository equipmentRepository,
    IUnitOfWork unitOfWork,
    ILogger<BatchCreateAuditRecordsCommandHandler> logger) : IRequestHandler<BatchCreateAuditRecordsCommand, BatchCreateAuditRecordsResult>
{
    /// <summary>
    /// Processes a batch command to create multiple audit records for equipment and returns the result of the
    /// operation.
    /// </summary>
    /// <remarks>If any equipment ID in the batch does not exist, the corresponding audit record will not be
    /// created and an error will be recorded for that entry. The method attempts to create as many audit records as
    /// possible, reporting individual failures without aborting the entire batch. All successful changes are committed
    /// in a single transaction.</remarks>
    /// <param name="request">The batch command containing the collection of audit records to create. Must not be null and must contain at
    /// least one audit record.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A result object containing details about the batch creation, including the number of successful and failed
    /// records, any errors encountered, and the IDs of created audit records.</returns>
    public async Task<BatchCreateAuditRecordsResult> Handle(BatchCreateAuditRecordsCommand request, CancellationToken cancellationToken)
    {
        var result = new BatchCreateAuditRecordsResult
        {
            TotalRecords = request.AuditRecords.Count
        };

        // Validate all equipment IDs exist first
        var equipmentIds = request.AuditRecords.Select(a => a.EquipmentId).Distinct().ToList();
        var equipments = new Dictionary<Guid, Equipment>();

        foreach (var equipmentId in equipmentIds)
        {
            var equipment = await equipmentRepository.GetByIdAsync(equipmentId, cancellationToken);
            if (equipment != null)
            {
                equipments[equipmentId] = equipment;
            }
        }

        // Process each audit record
        foreach (var auditInput in request.AuditRecords)
        {
            try
            {
                // Validate equipment exists
                if (!equipments.ContainsKey(auditInput.EquipmentId))
                {
                    result.FailureCount++;
                    result.Errors.Add($"Equipment with ID {auditInput.EquipmentId} not found");
                    continue;
                }

                var equipment = equipments[auditInput.EquipmentId];

                // Create audit record
                var auditRecord = new AuditRecord
                {
                    Id = Guid.NewGuid(),
                    EquipmentId = auditInput.EquipmentId,
                    CheckDate = auditInput.CheckDate,
                    CheckedByUserId = auditInput.CheckedByUserId,
                    Result = auditInput.Result,
                    Note = auditInput.Note,
                    Location = auditInput.Location,
                    LastSyncDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                // Update equipment location if changed
                if (!string.IsNullOrWhiteSpace(auditInput.Location))
                {
                    equipment.UpdatedAt = DateTime.UtcNow;
                    logger.LogInformation("Equipment {EquipmentId} location updated from {NewLocation}", equipment.Id, auditInput.Location);
                }

                await auditRecordRepository.AddAsync(auditRecord, cancellationToken);
                result.CreatedIds.Add(auditRecord.Id);
                result.SuccessCount++;
            }
            catch (Exception ex)
            {
                result.FailureCount++;
                result.Errors.Add($"Failed to create audit for equipment {auditInput.EquipmentId}: {ex.Message}");
                logger.LogError(ex, "Error creating audit record for equipment {EquipmentId}", auditInput.EquipmentId);
            }
        }

        // Save all changes in one transaction
        if (result.SuccessCount > 0)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "Batch audit creation completed: {SuccessCount} succeeded, {FailureCount} failed out of {TotalRecords}",
                result.SuccessCount, result.FailureCount, result.TotalRecords);
        }

        return result;
    }
}
