using Core.Domain.Events;

namespace Nutrition.Domain.Events
{
    public class MealFoodRemovedEvent : DomainEvent
    {
        public int DiaryId { get; }
        public int TotalCalories { get; }

        public MealFoodRemovedEvent(int diaryId, int totalCalories)
        {
            DiaryId = diaryId;
            TotalCalories = totalCalories;
        }
    }
}
