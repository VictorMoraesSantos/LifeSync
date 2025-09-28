using BuildingBlocks.CQRS.Notification;
using BuildingBlocks.CQRS.Publisher;
using EmailSender.Application.Contracts;
using EmailSender.Application.DTO;
using EmailSender.Domain.Entities;
using EmailSender.Domain.Events;
using Notification.Domain.Repositories;

namespace EmailSender.Application.EventHandlers
{
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredIntegrationEvent>
    {
        private readonly IEmailService _emailSender;
        private readonly IPublisher _publisher;
        private readonly IEmailMessageRepository emailMessageRepository;

        public UserRegisteredEventHandler(IEmailService emailSender, IPublisher publisher, IEmailMessageRepository emailMessageRepository)
        {
            _emailSender = emailSender;
            _publisher = publisher;
            this.emailMessageRepository = emailMessageRepository;
        }

        public async Task Handle(UserRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var dto = new EmailMessageDTO(
                To: notification.Email,
                Subject: "Welcome!",
                Body: "Thanks for registering.");

            var emailMessage = new EmailMessage("no-reply@test.local", dto.To, dto.Subject, dto.Body);
            await emailMessageRepository.Create(emailMessage);

            await _emailSender.SendEmailAsync(dto);

            var emailSentEvent = new EmailSentEvent(notification.Email, DateTime.Now);

            await _publisher.Publish(emailSentEvent, cancellationToken);
        }
    }
}
