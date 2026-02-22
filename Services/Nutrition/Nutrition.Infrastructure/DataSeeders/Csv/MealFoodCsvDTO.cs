namespace Nutrition.Infrastructure.DataSeeders.Csv
{
    public class MealFoodCsvDTO
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public int Calories { get; set; }
        public decimal? Protein { get; set; }
        public decimal? Lipids { get; set; }
        public decimal? Carbohydrates { get; set; }
        public decimal? Calcium { get; set; }
        public decimal? Magnesium { get; set; }
        public decimal? Iron { get; set; }
        public decimal? Sodium { get; set; }
        public decimal? Potassium { get; set; }
        public int Quantity { get; set; }
    }
}
