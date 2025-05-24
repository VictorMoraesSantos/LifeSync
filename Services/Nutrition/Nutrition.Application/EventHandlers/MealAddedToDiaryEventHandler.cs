using MediatR;
using Nutrition.Domain.Entities;
using Nutrition.Domain.Events;
using Nutrition.Domain.Repositories;

namespace Nutrition.Application.EventHandlers
{
    public class MealAddedToDiaryEventHandler : INotificationHandler<MealAddedToDiaryEvent>
    {
        private readonly IDailyProgressRepository _dailyProgressRepository;
        private readonly IMealRepository _mealRepository;

        public MealAddedToDiaryEventHandler(IDailyProgressRepository dailyProgressRepository, IMealRepository mealRepository)
        {
            _dailyProgressRepository = dailyProgressRepository;
            _mealRepository = mealRepository;
        }

        public async Task Handle(MealAddedToDiaryEvent notification, CancellationToken cancellationToken)
        {
            Meal? meal = await _mealRepository.GetById(notification.MealId, cancellationToken);
            if (meal == null) return;

            DailyProgress? dailyProgress = await _dailyProgressRepository.GetByUserIdAndDateAsync(notification.UserId, notification.Date, cancellationToken);
            if (dailyProgress == null) return;

            dailyProgress.SetConsumed(meal.TotalCalories, dailyProgress.LiquidsConsumedMl);
            await _dailyProgressRepository.Update(dailyProgress, cancellationToken);
        }
    }
}
