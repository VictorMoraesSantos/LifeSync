using Nutrition.Domain.Entities;

namespace Nutrition.UnitTests.Helpers.Builders
{
    public class MealFoodBuilder
    {
        private int _mealId = 1;
        private int _foodId = 1;
        private int _quantity = 100;

        public MealFoodBuilder WithMealId(int mealId) { _mealId = mealId; return this; }
        public MealFoodBuilder WithFoodId(int foodId) { _foodId = foodId; return this; }
        public MealFoodBuilder WithQuantity(int quantity) { _quantity = quantity; return this; }

        public MealFood Build()
        {
            return new MealFood(_mealId, _foodId, _quantity);
        }
    }
}
