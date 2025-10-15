using Notification.Application.Contracts;

namespace Notification.Application.Factories
{
    public class EmailEventStrategyFactory
    {
        private readonly Dictionary<string, IEmailEventStrategy> _strategies;

        public EmailEventStrategyFactory(IEnumerable<IEmailEventStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.EventType);
        }

        public IEmailEventStrategy? GetStrategy(string eventType)
        {
            _strategies.TryGetValue(eventType, out var strategy);
            return strategy;
        }
    }
}
