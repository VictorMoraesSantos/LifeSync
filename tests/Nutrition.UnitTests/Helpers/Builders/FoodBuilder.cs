using Nutrition.Domain.Entities;

namespace Nutrition.UnitTests.Helpers.Builders
{
    public class FoodBuilder
    {
        private string _name = "Arroz Branco";
        private int _calories = 130;
        private decimal? _protein = 2.7m;
        private decimal? _lipids = 0.3m;
        private decimal? _carbohydrates = 28.2m;
        private decimal? _calcium = 10m;
        private decimal? _magnesium = 12m;
        private decimal? _iron = 0.2m;
        private decimal? _sodium = 1m;
        private decimal? _potassium = 35m;

        public FoodBuilder WithName(string name) { _name = name; return this; }
        public FoodBuilder WithCalories(int calories) { _calories = calories; return this; }
        public FoodBuilder WithProtein(decimal? protein) { _protein = protein; return this; }
        public FoodBuilder WithLipids(decimal? lipids) { _lipids = lipids; return this; }
        public FoodBuilder WithCarbohydrates(decimal? carbohydrates) { _carbohydrates = carbohydrates; return this; }
        public FoodBuilder WithCalcium(decimal? calcium) { _calcium = calcium; return this; }
        public FoodBuilder WithMagnesium(decimal? magnesium) { _magnesium = magnesium; return this; }
        public FoodBuilder WithIron(decimal? iron) { _iron = iron; return this; }
        public FoodBuilder WithSodium(decimal? sodium) { _sodium = sodium; return this; }
        public FoodBuilder WithPotassium(decimal? potassium) { _potassium = potassium; return this; }

        public Food Build()
        {
            return new Food(
                _name, _calories, _protein, _lipids, _carbohydrates,
                _calcium, _magnesium, _iron, _sodium, _potassium);
        }
    }
}
