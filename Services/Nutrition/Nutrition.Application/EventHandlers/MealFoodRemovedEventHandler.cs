using MediatR;
using Nutrition.Domain.Events;
using Nutrition.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutrition.Application.EventHandlers
{
    public class MealFoodRemovedEventHandler : INotificationHandler<MealFoodRemovedEvent>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IDailyProgressRepository _dailyProgressRepository;

        public MealFoodRemovedEventHandler(IDiaryRepository diaryRepository, IDailyProgressRepository dailyProgressRepository)
        {   
            _diaryRepository = diaryRepository;
            _dailyProgressRepository = dailyProgressRepository;
        }

        public async Task Handle(MealFoodRemovedEvent notification, CancellationToken cancellationToken)
        {
            var diary = await _diaryRepository.GetById(notification.DiaryId, cancellationToken);
            if (diary == null) return;

            int totalCalories = diary.Meals.Sum(m => m.TotalCalories);

            var dailyProgress = await _dailyProgressRepository.GetByUserIdAndDateAsync(diary.UserId, diary.Date, cancellationToken);
            if (dailyProgress == null) return;

            dailyProgress.SetConsumed(diary.TotalCalories, dailyProgress.LiquidsConsumedMl);
            await _dailyProgressRepository.Update(dailyProgress, cancellationToken);
        }
    }
}
