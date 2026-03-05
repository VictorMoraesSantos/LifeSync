using Core.Domain.Events;

namespace Nutrition.Domain.Events
{
    public class LiquidChangedEvent : DomainEvent
    {
        public int DiaryId { get; }

        public LiquidChangedEvent(int diaryId)
        {
            DiaryId = diaryId;
        }
    }
}
