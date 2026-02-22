using Core.Application.DTO;

namespace Nutrition.Application.DTOs.MealFood
{
    public record MealFoodDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int Code,
        string Name,
        int Calories,
        decimal? Protein,
        decimal? Lipids,
        decimal? Carbohydrates,
        decimal? Calcium,
        decimal? Magnesium,
        decimal? Iron,
        decimal? Sodium,
        decimal? Potassium,
        int? Quantity,
        decimal? TotalCalories
        ) : DTOBase(Id, CreatedAt, UpdatedAt);
}
