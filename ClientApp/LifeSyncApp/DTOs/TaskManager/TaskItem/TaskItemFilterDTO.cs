using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.DTOs.TaskManager.TaskItem
{
    public record TaskItemFilterDTO(
        int? Id = null,
        int? UserId = null,
        string? TitleContains = null,
        Status? Status = null,
        Priority? Priority = null,
        DateOnly? DueDate = null,
        int? LabelId = null,
        DateOnly? CreatedAt = null,
        DateOnly? UpdatedAt = null,
        bool? IsDeleted = null,
        string? SortBy = null,
        bool? SortDesc = null,
        int? Page = null,
        int? PageSize = null);
}
