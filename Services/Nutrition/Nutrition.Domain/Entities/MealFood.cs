using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Errors;

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

        public void Update(string name, int quantity, int caloriesPerUnit)
        {
            SetName(name);
            SetQuantity(quantity);
            SetCaloriesPerUnit(caloriesPerUnit);

            MarkAsUpdated();
        }

        public void SetMeal(int mealId)
        {
            if (mealId <= 0)
                throw new DomainException(MealFoodErrors.InvalidMealId);
            MealId = mealId;
        }

        private void SetName(string name)
        {
            Validate(name);
            Name = name;
        }

        private void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException(MealFoodErrors.InvalidQuantity);
            Quantity = quantity;
        }

        private void SetCaloriesPerUnit(int caloriesPerUnit)
        {
            if (caloriesPerUnit < 0)
                throw new DomainException(MealFoodErrors.NegativeCalories);
            CaloriesPerUnit = caloriesPerUnit;
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{nameof(value)} cannot be empty.");
        }
    }
}
