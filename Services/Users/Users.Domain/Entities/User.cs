using Microsoft.AspNetCore.Identity;
using Users.Domain.ValueObjects;

namespace Users.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public Name Name { get; private set; }
        public Contact Contact { get; private set; }
        public DateOnly? BirthDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        protected User() { }

        public User(Name name, Contact contact)
        {
            Name = name;
            Contact = contact;
            Email = contact.Email;
            UserName = contact.Email;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void UpdateProfile(Name name, Contact contact)
        {
            Name = name;
            Contact = contact;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }
    }
}