using BuildingBlocks.CQRS.Notification;
using EmailSender.Domain.Events;
using Notification.Application.Contracts;
using Notification.Domain.Enums;

namespace EmailSender.Application.EventHandlers
{
    public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredIntegrationEvent>
    {
        private readonly IProcessEmailEventUseCase _useCase;

        public UserRegisteredEventHandler(IProcessEmailEventUseCase useCase)
        {
            _useCase = useCase;
        }

        public async Task Handle(UserRegisteredIntegrationEvent notification, CancellationToken cancellationToken)
        {
            await _useCase.HandleAsync(EmailEventTypes.UserRegistered, notification.Email, cancellationToken);
        }
    }
}