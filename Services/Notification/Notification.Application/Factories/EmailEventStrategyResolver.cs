using Notification.Application.Contracts;

namespace Notification.Application.Factories
{
    public class EmailEventStrategyResolver : IEmailEventStrategyResolver
    {
        private readonly IReadOnlyDictionary<string, IEmailEventStrategy> _strategies;

        public EmailEventStrategyResolver(IEnumerable<IEmailEventStrategy> strategies)
        {
            _strategies = strategies.ToDictionary(s => s.EventType, StringComparer.OrdinalIgnoreCase);
        }

        public IEmailEventStrategy? Resolve(string eventType)
        {
            _strategies.TryGetValue(eventType, out var strategy);
            return strategy;
        }
    }
}
