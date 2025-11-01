using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.Diary
{
    public record DiaryQueryFilterDTO(
        int? Id,
        int? UserId,
        int? TotalCaloriesEquals,
        int? TotalCaloriesGreaterThan,
        int? TotalCaloriesLessThan,
        int? TotalLiquidsMlEquals,
        int? TotalLiquidsMlGreaterThan,
        int? TotalLiquidsMlLessThan,
        int? MealId,
        int? LiquidId,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize
    ) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
