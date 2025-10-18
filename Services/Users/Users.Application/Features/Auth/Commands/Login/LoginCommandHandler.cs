using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.DTOs.Auth;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.Login
{
    public class LogInCommandHandler : ICommandHandler<LoginCommand, AuthResult>
    {
        private readonly IAuthService _authService;
        private readonly ITokenGenerator _tokenGenerator;

        public LogInCommandHandler(IAuthService identityService, ITokenGenerator tokenGenerator)
        {
            _authService = identityService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<AuthResult>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var userResult = await _authService.LoginAsync(command.Email, command.Password);
            if (!userResult.IsSuccess)
                return Result.Failure<AuthResult>(userResult.Error!);

            var accessTokenResult = _tokenGenerator.GenerateToken(
                userResult.Value!.Id,
                userResult.Value.Email,
                userResult.Value.Roles,
                cancellationToken);
            if (!accessTokenResult.IsSuccess)
                return Result.Failure<AuthResult>(accessTokenResult.Error!);

            var refreshTokenResult = _tokenGenerator.GenerateRefreshToken();
            if (!refreshTokenResult.IsSuccess)
                return Result.Failure<AuthResult>(refreshTokenResult.Error!);

            await _authService.UpdateRefreshTokenAsync(userResult.Value.Id, refreshTokenResult.Value!);

            return Result.Success(new AuthResult(accessTokenResult.Value!, refreshTokenResult.Value!, userResult.Value));
        }
    }
}
