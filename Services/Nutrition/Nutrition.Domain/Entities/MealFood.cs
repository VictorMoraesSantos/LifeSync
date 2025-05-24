using Core.Domain.Entities;

namespace Nutrition.Domain.Entities
{
    public class MealFood : BaseEntity<int>
    {
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public int CaloriesPerUnit { get; private set; }
        public int MealId { get; private set; }
        public int TotalCalories => Quantity * CaloriesPerUnit;

        public MealFood(string name, int quantity, int caloriesPerUnit)
        {
            SetName(name);
            SetQuantity(quantity);
            SetCaloriesPerUnit(caloriesPerUnit);
        }

        public void SetMeal(int mealId)
        {
            if (mealId <= 0)
                throw new ArgumentOutOfRangeException(nameof(mealId), "MealId must be positive.");
            MealId = mealId;
        }

        public void SetName(string name)
        {
            Validate(name);
            Name = name;
        }

        public void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
            Quantity = quantity;
        }

        public void SetCaloriesPerUnit(int caloriesPerUnit)
        {
            if (caloriesPerUnit < 0)
                throw new ArgumentOutOfRangeException(nameof(caloriesPerUnit), "Calories per unit cannot be negative.");
            CaloriesPerUnit = caloriesPerUnit;
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(value)} cannot be empty.");
        }
    }
}
