using Core.Domain.Exceptions;
using System.Text.RegularExpressions;
using Users.Domain.Errors;

namespace Users.Domain.ValueObjects
{
    public class Contact
    {
        public string Email { get; private set; }

        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public Contact(string? email = null)
        {
            if (email != null && !EmailRegex.IsMatch(email))
                throw new DomainException(ContactErrors.InvalidFormat);

            Email = email;
        }
    }
}
