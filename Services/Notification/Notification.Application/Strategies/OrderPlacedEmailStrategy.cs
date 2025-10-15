using EmailSender.Application.DTO;
using Notification.Application.Contracts;
using Notification.Domain.Enums;

namespace Notification.Application.Strategies
{
    public class OrderPlacedEmailStrategy : IEmailEventStrategy
    {
        public string EventType => EmailEventTypes.OrderPlaced;

        public EmailMessageDTO CreateEmail(string eventData)
        {
            var dto = new EmailMessageDTO(
                To: eventData,
                Subject: "Order Confirmation",
                Body: "Your order has been placed.");

            return dto;
        }
    }
}
