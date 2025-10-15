using EmailSender.Application.DTO;
using Notification.Application.Contracts;
using Notification.Domain.Enums;

namespace Notification.Application.Strategies
{
    public class UserRegisteredEmailStrategy : IEmailEventStrategy
    {
        public string EventType => EmailEventTypes.UserRegistered;

        public EmailMessageDTO CreateEmail(string eventData)
        {
            var dto = new EmailMessageDTO(
                    To: eventData,
                    Subject: "Welcome!",
                    Body: "Thanks for registering.");

            return dto;
        }
    }
}
