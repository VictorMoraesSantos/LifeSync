using Core.Domain.Entities;
using Core.Domain.Events;
using Microsoft.AspNetCore.Identity;
using Users.Domain.ValueObjects;

namespace Users.Domain.Entities
{
    public class User : IdentityUser<int>, IBaseEntity<int>
    {
        public Name Name { get; private set; }
        public Contact Contact { get; private set; }
        public DateOnly? BirthDate { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
        private readonly List<IDomainEvent> _domainEvents = new();
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        protected User() { }

        public User(Name name, Contact contact)
        {
            Name = name;
            Contact = contact;
            Email = contact.Email;
            UserName = contact.Email;
        }

        public void UpdateProfile(Name name, Contact contact)
        {
            Name = name;
            Contact = contact;
            MarkAsUpdated();
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkAsUpdated();
        }

        public void Activate()
        {
            IsActive = true;
            MarkAsUpdated();
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            MarkAsUpdated();
        }

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
            MarkAsUpdated();
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}