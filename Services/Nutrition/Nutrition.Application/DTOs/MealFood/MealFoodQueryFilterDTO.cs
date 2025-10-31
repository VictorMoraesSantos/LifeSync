using Core.Application.DTOs;

namespace Nutrition.Application.DTOs.MealFood
{
    public record MealFoodQueryFilterDTO(
        int? Id,
        string? NameContains,
        int? Quantity,
        int? CaloriesPerUnitEqual,
        int? CaloriesPerUnitGreaterThen,
        int? CaloriesPerUnitLessThen,
        int? MealId,
        int? TotalCaloriesEqual,
        int? TotalCaloriesGreaterThen,
        int? TotalCaloriesLessThen,
        DateOnly? CreatedAt,
        DateOnly? UpdatedAt,
        bool? IsDeleted,
        string? SortBy,
        bool? SortDesc,
        int? Page,
        int? PageSize
    ) : DomainQueryFilterDTO(CreatedAt, UpdatedAt, IsDeleted, SortBy, SortDesc, Page, PageSize);
}
