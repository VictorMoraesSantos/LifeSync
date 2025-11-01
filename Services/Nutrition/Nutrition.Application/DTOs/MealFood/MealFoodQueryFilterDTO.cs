using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.MealFood
{
    public record MealFoodQueryFilterDTO(
        int? Id,
        string? NameContains,
        int? Quantity,
        int? CaloriesPerUnitEquals,
        int? CaloriesPerUnitGreaterThan,
        int? CaloriesPerUnitLessThan,
        int? MealId,
        int? TotalCaloriesEquals,
        int? TotalCaloriesGreaterThan,
        int? TotalCaloriesLessThan,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize
    ) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
