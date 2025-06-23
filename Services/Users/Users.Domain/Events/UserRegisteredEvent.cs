using BuildingBlocks.Messaging.Abstractions;

namespace Users.Domain.Events
{
    public class UserRegisteredEvent : IntegrationEvent
    {
        public int UserId { get; set; }
        public string Email { get; }

        public UserRegisteredEvent(int userId,string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}
