using BuildingBlocks.CQRS.Notification;

namespace BuildingBlocks.Messaging.Abstractions
{
    public abstract class IntegrationEvent : INotification
    {
        public Guid Id { get; private set; }
        public DateTime CreationDate { get; }

        protected IntegrationEvent()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }
    }
}
