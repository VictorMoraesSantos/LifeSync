using MediatR;
using Users.Application.Interfaces;

namespace Users.Application.Users.Commands.LogOut
{
    public class SignOutCommandHandler : IRequestHandler<SignOutCommand, Unit>
    {
        private readonly IAuthService _authService;

        public SignOutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Unit> Handle(SignOutCommand command, CancellationToken cancellationToken)
        {
            await _authService.SignOutAsync(command.User);
            return Unit.Value;
        }
    }
}
