using EmailSender.Application.DTO;

namespace Notification.Application.Contracts
{
    public interface IEmailEventStrategy
    {
        public string EventType { get; }
        EmailMessageDTO CreateEmail(string eventData);
    }
}
