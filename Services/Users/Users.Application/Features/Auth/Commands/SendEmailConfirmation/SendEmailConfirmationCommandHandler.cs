using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.DTOs.Email;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.SendEmailConfirmation
{
    public class SendEmailConfirmationCommandHandler : ICommandHandler<SendEmailConfirmationCommand>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public SendEmailConfirmationCommandHandler(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        public async Task<Result> Handle(SendEmailConfirmationCommand command, CancellationToken cancellationToken)
        {
            var tokenResult = await _authService.SendEmailConfirmationAsync(command.Email);
            if (!tokenResult.IsSuccess)
                return Result.Failure(tokenResult.Error!);

            var dto = new EmailMessageDTO(
                To: command.Email,
                Subject: "Confirmação de E-mail",
                Body: null,
                IsHtml: true,
                Attachments: null,
                Token: tokenResult.Value,
                CallbackUrl: null);

            var result = await _emailService.SendConfirmationEmailAsync(dto);
            if (!result.IsSuccess)
                return Result.Failure(result.Error!);

            return Result.Success();
        }
    }
}
