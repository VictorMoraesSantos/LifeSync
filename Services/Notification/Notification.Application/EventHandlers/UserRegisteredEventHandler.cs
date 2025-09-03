using BuildingBlocks.CQRS.Notification;
using BuildingBlocks.CQRS.Publisher;
using EmailSender.Application.Contracts;
using EmailSender.Application.DTO;
using EmailSender.Domain.Events;

namespace EmailSender.Application.EventHandlers
{
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredIntegrationEvent>
    {
        private readonly IEmailSender _emailSender;
        private readonly IPublisher _publisher;

        public UserRegisteredEventHandler(IEmailSender emailSender, IPublisher publisher)
        {
            _emailSender = emailSender;
            _publisher = publisher;
        }

        public async Task Handle(UserRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var email = new EmailMessageDTO(
                To: notification.Email,
                Subject: "Welcome!",
                Body: "Thanks for registering.");

            await _emailSender.SendEmailAsync(email);

            var emailSentEvent = new EmailSentEvent(notification.Email, DateTime.Now);

            await _publisher.Publish(emailSentEvent, cancellationToken);
        }
    }
}
