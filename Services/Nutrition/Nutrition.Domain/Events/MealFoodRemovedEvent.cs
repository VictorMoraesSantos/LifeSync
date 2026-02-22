using Core.Domain.Events;

namespace Nutrition.Domain.Events
{
    public class MealFoodRemovedEvent : DomainEvent
    {
        public int DiaryId { get; }
        public decimal TotalCalories { get; }

        public MealFoodRemovedEvent(int diaryId, decimal totalCalories)
        {
            DiaryId = diaryId;
            TotalCalories = totalCalories;
        }
    }
}
