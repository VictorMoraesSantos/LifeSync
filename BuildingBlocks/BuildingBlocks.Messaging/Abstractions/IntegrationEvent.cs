namespace BuildingBlocks.Messaging.Abstractions
{
    public abstract class IntegrationEvent
    {
        public int Id { get; private set; }
        public DateTime CreationDate { get; }

        protected IntegrationEvent(int id)
        {
            Id = id;
            CreationDate = DateTime.UtcNow;
        }
    }
}
