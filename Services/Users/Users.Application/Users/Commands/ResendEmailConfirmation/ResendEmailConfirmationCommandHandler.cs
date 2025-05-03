using MediatR;
using Users.Application.Interfaces;

namespace Users.Application.Users.Commands.ResendEmailConfirmation
{
    public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand, bool>
    {
        private readonly IAuthService _authService;

        public ResendEmailConfirmationCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(ResendEmailConfirmationCommand command, CancellationToken cancellationToken)
        {
            bool result = await _authService.SendEmailConfirmationAsync(command.Email);

            return result;
        }
    }
}
