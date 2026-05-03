using System.Security.Claims;
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Contracts;
using Users.Application.DTOs.Auth;
using Users.Application.Features.Auth.Commands.ExternalLogin;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.GoogleCallback
{
    public class GoogleCallbackCommandHandler : ICommandHandler<GoogleCallbackCommand, AuthResult>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IAuthService _authService;
        private readonly ITokenGenerator _tokenGenerator;

        public GoogleCallbackCommandHandler(
            IGoogleAuthService googleAuthService,
            IAuthService authService,
            ITokenGenerator tokenGenerator)
        {
            _googleAuthService = googleAuthService;
            _authService = authService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<AuthResult>> Handle(GoogleCallbackCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(command.Code))
                return Result<AuthResult>.Failure(Error.Problem("Authorization code is required."));

            var idTokenResult = await _googleAuthService.ExchangeCodeForIdTokenAsync(command.Code, cancellationToken);
            if (!idTokenResult.IsSuccess)
                return Result<AuthResult>.Failure(idTokenResult.Error);

            var payload = await _googleAuthService.ValidateIdTokenAsync(idTokenResult.Value!);
            if (!payload.IsSuccess)
                return Result<AuthResult>.Failure(payload.Error);

            var firstName = payload.Value?.GivenName ?? payload.Value?.Name ?? "User";
            var lastName = payload.Value?.FamilyName ?? "";

            var userResult = await _authService.ExternalLoginAsync(
                payload.Value?.Email, firstName, lastName, "Google", payload.Value?.Subject);
            if (!userResult.IsSuccess)
                return Result<AuthResult>.Failure(userResult.Error);

            var extra = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userResult.Value!.Id),
                new(ClaimTypes.Name, userResult.Value.FullName ?? string.Empty),
                new(ClaimTypes.GivenName, userResult.Value.FirstName ?? string.Empty),
                new(ClaimTypes.Surname, userResult.Value.LastName ?? string.Empty)
            };

            var accessTokenResult = _tokenGenerator.GenerateToken(
                userResult.Value!.Id,
                userResult.Value.Email,
                userResult.Value.Roles,
                cancellationToken,
                extra);
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
