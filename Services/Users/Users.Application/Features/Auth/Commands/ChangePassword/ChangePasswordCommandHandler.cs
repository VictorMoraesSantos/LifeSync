using BuildingBlocks.CQRS.Request;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IAuthService _authService;

        public ChangePasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            await _authService.ChangePasswordAsync(command.User, command.CurrentPassword, command.NewPassword);
        }
    }
}
