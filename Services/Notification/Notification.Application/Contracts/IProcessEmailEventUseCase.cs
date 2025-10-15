namespace Notification.Application.Contracts
{
    public interface IProcessEmailEventUseCase
    {
        Task HandleAsync(string eventType, string eventData, CancellationToken cancellationToken = default);
    }
}
