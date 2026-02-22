using Core.Domain.Events;

namespace Nutrition.Domain.Events
{
    public class MealFoodAddedEvent : DomainEvent
    {
        public int DiaryId { get; }
        public decimal TotalCalories { get; }

        public MealFoodAddedEvent(int diaryId, decimal totalCalories)
        {
            DiaryId = diaryId;
            TotalCalories = totalCalories;
        }
    }
}