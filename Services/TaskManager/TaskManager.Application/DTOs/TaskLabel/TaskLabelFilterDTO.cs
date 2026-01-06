using Core.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskLabel
{
    public record TaskLabelFilterDTO(
        int? Id,
        int? UserId,
        int? ItemId,
        string? NameContains,
        LabelColor? LabelColor,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize)
        : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
