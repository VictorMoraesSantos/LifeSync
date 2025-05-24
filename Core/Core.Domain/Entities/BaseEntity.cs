using Core.Domain.Events;

namespace Core.Domain.Entities
{
    public interface IBaseEntity<T>
    {
        T Id { get; }
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
        bool IsDeleted { get; }
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        void MarkAsUpdated();
        void MarkAsDeleted();
        void AddDomainEvent(IDomainEvent domainEvent);
        void ClearDomainEvents();
    }

    public abstract class BaseEntity<T> : IBaseEntity<T>
    {
        public T Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }
        public bool IsDeleted { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public virtual void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public virtual void MarkAsDeleted()
        {
            if (IsDeleted)
                return;
            IsDeleted = true;
            MarkAsUpdated();
        }

        public virtual void AddDomainEvent(IDomainEvent domainEvent)
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));
            _domainEvents.Add(domainEvent);
        }

        public virtual void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BaseEntity<T> other))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return EqualityComparer<T>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return Id == null ? 0 : Id.GetHashCode();
        }
    }
}
