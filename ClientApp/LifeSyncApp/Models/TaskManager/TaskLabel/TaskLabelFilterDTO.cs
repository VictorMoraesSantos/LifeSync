namespace LifeSyncApp.Models.TaskManager
{
    public record TaskLabelFilterDTO(
        int? Id = null,
        int? UserId = null,
        DateOnly? CreatedAt = null,
        DateOnly? UpdatedAt = null,
        bool? IsDeleted = null,
        string? SortBy = null,
        bool? SortDesc = null,
        int? Page = null,
        int? PageSize = null,
        int? ItemId = null,
        string? NameContains = null);
}