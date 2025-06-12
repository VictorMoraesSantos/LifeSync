using BuildingBlocks.Messaging.Abstractions;
using Core.Domain.Events;

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
