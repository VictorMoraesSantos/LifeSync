using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Errors;

namespace Nutrition.Domain.Entities
{
    public class MealFood : BaseEntity<int>
    {
        public int Code { get; private set; }
        public string Name { get; private set; }
        public int Calories { get; private set; }
        public decimal? Protein { get; set; }
        public decimal? Lipids { get; set; }
        public decimal? Carbohydrates { get; set; }
        public decimal? Calcium { get; set; }
        public decimal? Magnesium { get; set; }
        public decimal? Iron { get; set; }
        public decimal? Sodium { get; set; }
        public decimal? Potassium { get; set; }
        public int ?Quantity { get; private set; }
        public decimal TotalCalories => (decimal)(Quantity * Calories);

        public MealFood(
            int code,
            string name,
            int calories,
            decimal? protein,
            decimal? lipids,
            decimal? carbohydrates,
            decimal? calcium,
            decimal? magnesium,
            decimal? iron,
            decimal? sodium,
            decimal? potassium,
            int? quantity)
        {
            SetName(name);
            Code = code;
            Calories = calories;
            Protein = protein;
            Lipids = lipids;
            Carbohydrates = carbohydrates;
            Calcium = calcium;
            Magnesium = magnesium;
            Iron = iron;
            Sodium = sodium;
            Potassium = potassium;
            Quantity = quantity;
        }

        public void Update(
            int code,
            string name,
            int calories,
            decimal? protein,
            decimal? lipids,
            decimal? carbohydrates,
            decimal? calcium,
            decimal? magnesium,
            decimal? iron,
            decimal? sodium,
            decimal? potassium,
            int? quantity)
        {
            SetName(name);
            Code = code;
            Calories = calories;
            Protein = protein;
            Lipids = lipids;
            Carbohydrates = carbohydrates;
            Calcium = calcium;
            Magnesium = magnesium;
            Iron = iron;
            Sodium = sodium;
            Potassium = potassium;
            Quantity = quantity;

            MarkAsUpdated();
        }

        private void SetName(string name)
        {
            Validate(name);
            Name = name;
        }
            
        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(value)} cannot be empty.");
        }
    }
}
