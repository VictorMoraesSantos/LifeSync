using MediatR;
using Users.Application.Interfaces;

namespace Users.Application.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Unit> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(command.User);
            return Unit.Value;
        }
    }
}
