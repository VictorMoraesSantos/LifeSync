namespace Notification.Application.Contracts
{
    public interface IEmailEventStrategyResolver
    {
        IEmailEventStrategy? Resolve(string eventType);
    }
}
