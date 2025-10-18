using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.DTOs.Email;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        public async Task<Result> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var tokenResult = await _authService.SendPasswordResetAsync(command.Email);
            if (!tokenResult.IsSuccess)
                return Result.Failure(tokenResult.Error!);

            var dto = new EmailMessageDTO(
                To: command.Email,
                Subject: "Redefinição de Senha",
                Body: null,
                IsHtml: true,
                Attachments: null,
                Token: tokenResult.Value,
                CallbackUrl: null);

            await _emailService.SendForgotPasswordEmailAsync(dto);

            return Result.Success();
        }
    }
}
