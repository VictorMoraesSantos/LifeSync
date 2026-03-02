using Core.Domain.Entities;

namespace Nutrition.Domain.Entities
{
    public class Food : BaseEntity<int>
    {
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

        public Food(
            string name,
            int calories,
            decimal? protein,
            decimal? lipids,
            decimal? carbohydrates,
            decimal? calcium,
            decimal? magnesium,
            decimal? iron,
            decimal? sodium,
            decimal? potassium)
        {
            Name = name;
            Calories = calories;
            Protein = protein;
            Lipids = lipids;
            Carbohydrates = carbohydrates;
            Calcium = calcium;
            Magnesium = magnesium;
            Iron = iron;
            Sodium = sodium;
            Potassium = potassium;
        }
    }
}
