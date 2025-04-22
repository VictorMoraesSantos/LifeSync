using Microsoft.AspNetCore.Identity;

namespace Users.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string FullName => $"{FirstName} {LastName}";

        public DateOnly? BirthDate { get; private set; }

        public string DocumentNumber { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        public bool IsActive { get; private set; }

        protected User() { }

        public User(string firstName, string lastName, string email, string userName, string documentNumber = null, DateOnly? birthDate = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            UserName = userName;
            DocumentNumber = documentNumber;
            BirthDate = birthDate;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void UpdateProfile(string firstName, string lastName, string email, string documentNumber, DateOnly? birthDate)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            DocumentNumber = documentNumber;
            BirthDate = birthDate;
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