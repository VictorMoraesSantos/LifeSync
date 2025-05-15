using Core.Application.DTO;

namespace Nutrition.Application.DTOs.MealFood
{
    public record MealFoodDTO(
        int Id,
        int MealId,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        int Quantity,
        int CaloriesPerUnit,
        int TotalCalories
        ) : DTOBase(Id, CreatedAt, UpdatedAt);
}
