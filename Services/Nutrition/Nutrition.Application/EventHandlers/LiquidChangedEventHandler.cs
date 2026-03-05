using BuildingBlocks.CQRS.Notification;
using Nutrition.Domain.Events;
using Nutrition.Domain.Repositories;

namespace Nutrition.Application.EventHandlers
{
    public class LiquidChangedEventHandler : INotificationHandler<LiquidChangedEvent>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IDailyProgressRepository _dailyProgressRepository;

        public LiquidChangedEventHandler(
            IDiaryRepository diaryRepository,
            IDailyProgressRepository dailyProgressRepository)
        {
            _diaryRepository = diaryRepository;
            _dailyProgressRepository = dailyProgressRepository;
        }

        public async Task Handle(LiquidChangedEvent notification, CancellationToken cancellationToken)
        {
            var diary = await _diaryRepository.GetById(notification.DiaryId, cancellationToken);
            if (diary == null) return;

            var dailyProgress = await _dailyProgressRepository.GetByUserIdAndDateAsync(diary.UserId, diary.Date, cancellationToken);
            if (dailyProgress == null) return;

            int totalLiquidsMl = diary.Liquids.Sum(l => l.Quantity);
            dailyProgress.SetConsumed(dailyProgress.CaloriesConsumed, totalLiquidsMl);
            await _dailyProgressRepository.Update(dailyProgress, cancellationToken);
        }
    }
}
