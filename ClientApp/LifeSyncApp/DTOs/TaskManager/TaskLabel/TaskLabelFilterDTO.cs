using LifeSyncApp.Models.TaskManager.Enums;

namespace LifeSyncApp.DTOs.TaskManager.TaskLabel
{
    public record TaskLabelFilterDTO(
        int? Id = null,
        int? UserId = null,
        int? ItemId = null,
        string? NameContains = null,
        LabelColor? LabelColor = null,
        DateOnly? CreatedAt = null,
        DateOnly? UpdatedAt = null,
        bool? IsDeleted = null,
        string? SortBy = null,
        bool? SortDesc = null,
        int? Page = null,
        int? PageSize = null);
}
