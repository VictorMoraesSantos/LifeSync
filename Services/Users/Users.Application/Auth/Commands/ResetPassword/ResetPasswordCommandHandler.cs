using MediatR;
using Users.Application.Interfaces;

namespace Users.Application.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly IAuthService _authService;

        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<bool> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            bool result = await _authService.ResetPasswordAsync(command.UserId, command.Token, command.NewPassword);
            return result;
        }
    }
}
