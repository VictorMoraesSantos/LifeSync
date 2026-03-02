namespace LifeSyncApp.DTOs.Nutrition.Food
{
    public record FoodDTO(
        int Id,
        string Name,
        int Calories,
        decimal? Protein,
        decimal? Lipids,
        decimal? Carbohydrates,
        decimal? Calcium,
        decimal? Magnesium,
        decimal? Iron,
        decimal? Sodium,
        decimal? Potassium);
}
