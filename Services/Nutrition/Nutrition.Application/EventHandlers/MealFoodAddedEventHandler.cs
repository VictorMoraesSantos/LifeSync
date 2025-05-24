using MediatR;
using Nutrition.Domain.Events;
using Nutrition.Domain.Repositories;

namespace Nutrition.Application.EventHandlers
{
    public class MealFoodAddedEventHandler : INotificationHandler<MealFoodAddedEvent>
    {
        private readonly IMealRepository _mealRepository;
        private readonly IDiaryRepository _diaryRepository;
        private readonly IDailyProgressRepository _dailyProgressRepository;

        public MealFoodAddedEventHandler(
            IMealRepository mealRepository,
            IDiaryRepository diaryRepository,
            IDailyProgressRepository dailyProgressRepository)
        {
            _mealRepository = mealRepository;
            _diaryRepository = diaryRepository;
            _dailyProgressRepository = dailyProgressRepository;
        }

        public async Task Handle(MealFoodAddedEvent notification, CancellationToken cancellationToken)
        {
            var meal = await _mealRepository.GetById(notification.MealId, cancellationToken);
            if (meal == null) return;

            var diary = await _diaryRepository.GetById(meal.DiaryId, cancellationToken);
            if (diary == null) return;

            int totalCalories = diary.Meals.Sum(m => m.TotalCalories);

            var dailyProgress = await _dailyProgressRepository.GetByUserIdAndDateAsync(diary.UserId, diary.Date, cancellationToken);
            if (dailyProgress == null) return;

            dailyProgress.SetConsumed(diary.TotalCalories, dailyProgress.LiquidsConsumedMl);
            await _dailyProgressRepository.Update(dailyProgress, cancellationToken);
        }
    }
}