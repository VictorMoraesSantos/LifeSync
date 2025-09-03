using BuildingBlocks.Messaging.Abstractions;

namespace EmailSender.Domain.Events
{
    public class UserRegisteredIntegrationEvent : IntegrationEvent
    {
        public int UserId { get; }
        public string Email { get; }

        public UserRegisteredIntegrationEvent(int userId, string email) : base()
        {
            UserId = userId;
            Email = email;
        }
    }
}
