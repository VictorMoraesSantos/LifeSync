using BuildingBlocks.CQRS.Request;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.SendEmailConfirmation
{
    public class SendEmailConfirmationCommandHandler : IRequestHandler<SendEmailConfirmationCommand>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public SendEmailConfirmationCommandHandler(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        public async Task Handle(SendEmailConfirmationCommand command, CancellationToken cancellationToken)
        {
            string token = await _authService.SendEmailConfirmationAsync(command.Email);
            await _emailService.SendConfirmationEmailAsync(command.Email, token);
        }
    }
}
