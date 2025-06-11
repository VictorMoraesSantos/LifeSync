using BuildingBlocks.CQRS.Notification;
using EmailSender.Application.Contracts;
using EmailSender.Application.DTO;
using EmailSender.Domain.Events;

namespace EmailSender.Application.EventHandlers
{
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IEmailSender _emailSender;

        public UserRegisteredEventHandler(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            var email = new EmailMessageDTO(
                To: notification.Email,
                Subject: "Welcome!",
                Body: "Thanks for registering.");

            await _emailSender.SendEmailAsync(email);
        }
    }
}
