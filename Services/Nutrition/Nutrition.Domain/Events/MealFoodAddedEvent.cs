using Core.Domain.Events;

namespace Nutrition.Domain.Events
{
    public class MealFoodAddedEvent : DomainEvent
    {
        public int MealId { get; }
        public int TotalCalories { get; }

        public MealFoodAddedEvent(int mealId, int totalCalories)
        {
            MealId = mealId;
            TotalCalories = totalCalories;
        }
    }
}