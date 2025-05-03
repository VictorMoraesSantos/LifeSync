using System.Text.RegularExpressions;

namespace Users.Domain.ValueObjects
{
    public class Contact
    {
        public string Email { get; private set; }

        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public Contact(string? email = null)
        {
            if (email != null && !EmailRegex.IsMatch(email))
                throw new ArgumentException("Alternate email format is invalid.", nameof(email));

            Email = email;
        }
    }
}
