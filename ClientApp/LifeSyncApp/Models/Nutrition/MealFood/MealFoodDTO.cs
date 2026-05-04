namespace LifeSyncApp.Models.Nutrition.MealFood
{
    public record MealFoodDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        int MealId,
        int FoodId,
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
        int Quantity,
        decimal TotalCalories)
    {
        public decimal? ScaledProtein => Protein.HasValue ? Protein.Value * Quantity / 100m : null;
        public decimal? ScaledLipids => Lipids.HasValue ? Lipids.Value * Quantity / 100m : null;
        public decimal? ScaledCarbohydrates => Carbohydrates.HasValue ? Carbohydrates.Value * Quantity / 100m : null;
    }
}
