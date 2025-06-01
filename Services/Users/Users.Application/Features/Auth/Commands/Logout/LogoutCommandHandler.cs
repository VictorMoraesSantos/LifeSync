using BuildingBlocks.CQRS.Request;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            await _authService.LogoutAsync(command.User);
        }
    }
}
