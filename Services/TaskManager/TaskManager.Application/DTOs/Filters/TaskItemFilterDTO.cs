using Core.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.Filters
{
    public record TaskItemFilterDTO(
        int? UserId,
        string? TitleContains,
        Status? Status,
        Priority? Priority,
        DateOnly? DueDate,
        int? LabelId,
        int? Id,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize
    ) : DomainQueryFilterDTO<int?>(Id, CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
