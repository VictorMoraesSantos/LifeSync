using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Contracts;
using Users.Application.DTOs.Auth;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.ExternalLogin
{
    public class ExternalLoginCommandHandler : ICommandHandler<ExternalLoginCommand, AuthResult>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IAuthService _authService;
        private readonly ITokenGenerator _tokenGenerator;

        public ExternalLoginCommandHandler(IGoogleAuthService googleAuthService, IAuthService authService, ITokenGenerator tokenGenerator)
        {
            _googleAuthService = googleAuthService;
            _authService = authService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<AuthResult>> Handle(ExternalLoginCommand command, CancellationToken cancellationToken)
        {
            var payload = await _googleAuthService.ValidateIdTokenAsync(command.idToken);
            if (!payload.IsSuccess) return Result<AuthResult>.Failure(payload.Error);

            var firstName = payload.Value?.GivenName ?? payload.Value?.Name ?? "User";
            var lastName = payload.Value?.FamilyName ?? "";

            var userResult = await _authService.ExternalLoginAsync(
                payload.Value?.Email, firstName, lastName, "Google", payload.Value?.Subject);
            if (!userResult.IsSuccess)
                return Result<AuthResult>.Failure(userResult.Error);

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
