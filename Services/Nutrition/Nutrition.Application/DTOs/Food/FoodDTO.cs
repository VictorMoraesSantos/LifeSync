using Core.Application.DTO;

namespace Nutrition.Application.DTOs.Food
{
    public record FoodDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        int Calories,
        decimal? Protein,
        decimal? Lipids,
        decimal? Carbohydrates,
        decimal? Calcium,
        decimal? Magnesium,
        decimal? Iron,
        decimal? Sodium,
        decimal? Potassium)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
