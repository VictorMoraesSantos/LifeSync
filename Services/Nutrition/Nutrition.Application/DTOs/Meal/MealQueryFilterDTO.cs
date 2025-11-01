using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.Meal
{
    public record MealQueryFilterDTO(
        int? Id,
        string? NameContains,
        string? DescriptionContains,
        int? DiaryId,
        int? TotalCaloriesEquals,
        int? TotalCaloriesGreaterThan,
        int? TotalCaloriesLessThan,
        int? MealFoodId,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize
    ) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
