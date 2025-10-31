using Core.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskItem
{
    public record TaskItemFilterDTO(
        int? Id,
        int? UserId,
        string? TitleContains,
        Status? Status,
        Priority? Priority,
        DateOnly? DueDate,
        int? LabelId,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize)
        : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
