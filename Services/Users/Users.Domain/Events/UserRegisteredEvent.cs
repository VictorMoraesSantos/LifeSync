using BuildingBlocks.Messaging.Abstractions;

namespace Users.Domain.Events
{
    public class UserRegisteredEvent : IntegrationEvent
    {
        public string Email { get; }

        public UserRegisteredEvent(int id, string email) : base(id)
        {
            Email = email;
        }
    }
}
