using Core.Domain.Entities;
using Core.Domain.Exceptions;
using Nutrition.Domain.Errors;

namespace Nutrition.Domain.Entities
{
    public class MealFood : BaseEntity<int>
    {
        public int MealId { get; private set; }
        public int FoodId { get; private set; }
        public Food Food { get; private set; }
        public int Quantity { get; private set; }
        public decimal TotalCalories => Quantity * Food.Calories;

        public MealFood(
            int mealId,
            int FoodId,
            int quantity)
        {
            SetMealId(mealId);
            SetFoodId(FoodId);
            SetQuantity(quantity);
        }

        public void Update(
            int FoodId,
            int quantity)
        {
            SetFoodId(FoodId);
            SetQuantity(quantity);
            MarkAsUpdated();
        }

        private void SetMealId(int mealId)
        {
            if (mealId <= 0)
                throw new DomainException(MealFoodErrors.InvalidMealId);
            MealId = mealId;
        }

        private void SetFoodId(int foodId)
        {
            if (foodId <= 0)
                throw new DomainException(MealFoodErrors.InvalidFoodId);
            FoodId = foodId;
        }

        private void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException(MealFoodErrors.InvalidQuantity);
            Quantity = quantity;
        }
    }
}
