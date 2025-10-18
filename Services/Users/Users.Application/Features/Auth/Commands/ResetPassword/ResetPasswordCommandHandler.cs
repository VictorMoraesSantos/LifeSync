using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
    {
        private readonly IAuthService _authService;

        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<ResetPasswordResult>> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _authService.ResetPasswordAsync(command.UserId, command.Token, command.NewPassword);
            if (!result.IsSuccess)
                return Result.Failure<ResetPasswordResult>(result.Error!);

            return Result.Success(new ResetPasswordResult(result.Value));
        }
    }
}
