using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.Logout
{
    public class LogoutCommandHandler : ICommandHandler<LogoutCommand, LogoutResult>
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<LogoutResult>> Handle(LogoutCommand command, CancellationToken cancellationToken)
        {
            var result = await _authService.LogoutAsync(command.User);
            if (!result.IsSuccess)
                return Result.Failure<LogoutResult>(result.Error!);

            return Result.Success(new LogoutResult(result.IsSuccess));
        }
    }
}
