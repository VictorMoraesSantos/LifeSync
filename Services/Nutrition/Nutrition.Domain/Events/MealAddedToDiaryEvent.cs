using Core.Domain.Events;

namespace Nutrition.Domain.Events
{
    public class MealAddedToDiaryEvent : DomainEvent
    {
        public int UserId { get; }
        public DateOnly Date { get; }
        public int MealId { get; }

        public MealAddedToDiaryEvent(int userId, DateOnly date, int mealId)
        {
            UserId = userId;
            Date = date;
            MealId = mealId;
        }
    }
}