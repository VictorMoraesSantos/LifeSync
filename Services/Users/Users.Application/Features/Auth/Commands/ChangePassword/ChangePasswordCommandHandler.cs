using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
    {
        private readonly IAuthService _authService;

        public ChangePasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _authService.ChangePasswordAsync(command.User, command.CurrentPassword, command.NewPassword);
            if (!result.IsSuccess)
                return Result.Failure(result.Error!);

            return Result.Success();
        }
    }
}
