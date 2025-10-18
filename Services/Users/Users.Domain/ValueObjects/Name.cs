using Core.Domain.Exceptions;
using Users.Domain.Errors;

namespace Users.Domain.ValueObjects
{
    public class Name
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";

        public Name(string firstName, string lastName)
        {
            Validate(firstName);
            Validate(lastName);
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException(NameErrors.NullName);
        }
    }
}
