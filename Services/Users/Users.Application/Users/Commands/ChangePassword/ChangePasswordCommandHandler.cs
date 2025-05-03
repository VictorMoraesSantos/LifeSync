using MediatR;
using Users.Application.Interfaces;

namespace Users.Application.Users.Commands.ChangePassword
{
    public record ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, bool>
    {
        private readonly IAuthService _authService;
     
        public ChangePasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }
        
        public async Task<bool> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            bool result = await _authService.ChangePasswordAsync(command.User, command.CurrentPassword, command.NewPassword);
            return result;
        }
    }
}
