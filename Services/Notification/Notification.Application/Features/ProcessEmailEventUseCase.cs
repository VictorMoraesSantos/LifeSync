using EmailSender.Application.Contracts;
using Notification.Application.Contracts;

namespace EmailSender.Application.Features
{
    public class ProcessEmailEventUseCase : IProcessEmailEventUseCase
    {
        private readonly IEmailService _emailSender;
        private readonly IEmailEventStrategyResolver _strategyResolver;

        public ProcessEmailEventUseCase(IEmailService emailSender, IEmailEventStrategyResolver strategyResolver)
        {
            _emailSender = emailSender;
            _strategyResolver = strategyResolver;
        }

        public async Task HandleAsync(string eventType, string eventData, CancellationToken cancellationToken)
        {
            var strategy = _strategyResolver.Resolve(eventType);
            if (strategy == null)
                return;

            var email = strategy?.CreateEmail(eventData);

            await _emailSender.SendEmailAsync(email, cancellationToken);
        }
    }
}
