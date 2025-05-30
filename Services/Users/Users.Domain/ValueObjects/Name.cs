﻿namespace Users.Domain.ValueObjects
{
    public class Name
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
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
                throw new ArgumentException($"{nameof(value)} name cannot be empty.", nameof(value));
        }
    }
}
