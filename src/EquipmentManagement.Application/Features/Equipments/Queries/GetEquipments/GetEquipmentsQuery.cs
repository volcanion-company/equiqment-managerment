using EquipmentManagement.Application.Common.Models;
using EquipmentManagement.Application.Features.Equipments.DTOs;
using MediatR;

namespace EquipmentManagement.Application.Features.Equipments.Queries.GetEquipments;

public class GetEquipmentsQuery : IRequest<PagedResult<EquipmentDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
}
