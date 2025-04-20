namespace Core.Domain.Events
{
    public interface IDomainEvent
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
