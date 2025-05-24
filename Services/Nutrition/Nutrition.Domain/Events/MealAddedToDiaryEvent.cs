using Core.Domain.Events;
using MediatR;

namespace Nutrition.Domain.Events
{
    public class MealAddedToDiaryEvent : DomainEvent, INotification
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