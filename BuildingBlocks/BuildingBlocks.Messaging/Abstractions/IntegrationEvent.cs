using BuildingBlocks.CQRS.Notification;

namespace BuildingBlocks.Messaging.Abstractions
{
    public abstract class IntegrationEvent : INotification
    {
        public int Id { get; private set; }
        public DateTime CreationDate { get; }

        public IntegrationEvent()
        {
            CreationDate = DateTime.UtcNow;
        }

        public IntegrationEvent(int id)
        {
            Id = id;
            CreationDate = DateTime.UtcNow;
        }
    }
}
