using BuildingBlocks.CQRS.Notification;

namespace Core.Domain.Events
{
    public interface IDomainEvent : INotification
    {
        int Id { get; set; }
        DateTime OccuredOn { get; }
    }

    public class DomainEvent : IDomainEvent
    {
        public int Id { get; set; }
        public DateTime OccuredOn { get; protected set; }

        public DomainEvent()
        {
            OccuredOn = DateTime.Now;
        }
    }
}
