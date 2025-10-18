using BuildingBlocks.CQRS.Request;
using Users.Application.DTOs.Email;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        public async Task Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var tokenResult = await _authService.SendPasswordResetAsync(command.Email);
            if (!tokenResult.IsSuccess) return;

            var dto = new EmailMessageDTO(
                To: command.Email,
                Subject: "Redefinição de Senha",
                Body: null,
                IsHtml: true,
                Attachments: null,
                Token: tokenResult.Value,
                CallbackUrl: null
            );

            await _emailService.SendForgotPasswordEmailAsync(dto);
        }
    }
}
