using BuildingBlocks.CQRS.Notification;

namespace Core.Domain.Events
{
    public interface IDomainEvent : INotification
    {
        Guid Id { get; set; }
        DateTime OccuredOn { get; }
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime OccuredOn { get; protected set; } = DateTime.UtcNow;

        public DomainEvent()
        {
        }
    }
}
