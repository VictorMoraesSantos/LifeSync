using BuildingBlocks.CQRS.Request;
using Users.Application.DTOs.Email;
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
            var tokenResult = await _authService.SendEmailConfirmationAsync(command.Email);
            if (!tokenResult.IsSuccess) return;

            var dto = new EmailMessageDTO(
                To: command.Email,
                Subject: "Confirmação de E-mail",
                Body: null,
                IsHtml: true,
                Attachments: null,
                Token: tokenResult.Value,
                CallbackUrl: null
            );

            await _emailService.SendConfirmationEmailAsync(dto);
        }
    }
}
