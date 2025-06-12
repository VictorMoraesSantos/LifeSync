using Core.Domain.Events;

namespace EmailSender.Domain.Events
{
    public class UserRegisteredEvent : DomainEvent
    {
        public string Email { get; private set; }

        public UserRegisteredEvent(string email)
        {
            Email = email;
        }
    }
}
