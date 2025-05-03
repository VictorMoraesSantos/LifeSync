using MediatR;
using Users.Application.Interfaces;

namespace Users.Application.Users.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }

        public async Task<bool> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            string token = await _authService.SendPasswordResetAsync(command.Email);
            await _emailService.SendForgotPasswordEmailAsync(command.Email, token);

            return true;
        }
    }
}
