using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using BuildingBlocks.Results;
using RabbitMQ.Client;
using System.Security.Claims;
using Users.Application.DTOs.Auth;
using Users.Application.Interfaces;
using Users.Domain.Events;

namespace Users.Application.Features.Auth.Commands.SignUp
{
    public class SignUpCommandHandler : ICommandHandler<SignUpCommand, AuthResult>
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEventBus _eventBus;

        public SignUpCommandHandler(IAuthService identityService, ITokenGenerator tokenGenerator, IUserService userService, IEventBus eventBus)
        {
            _authService = identityService;
            _tokenGenerator = tokenGenerator;
            _userService = userService;
            _eventBus = eventBus;
        }

        public async Task<Result<AuthResult>> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            // Create user
            var userResult = await _authService.SignUpAsync(
                command.FirstName,
                command.LastName,
                command.Email,
                command.Password);

            if (!userResult.IsSuccess)
                return Result.Failure<AuthResult>(userResult.Error!);

            // Generate tokens with profile claims
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

            // Persist refresh token
            var updateRt = await _authService.UpdateRefreshTokenAsync(userResult.Value.Id, refreshTokenResult.Value!);
            if (!updateRt.IsSuccess)
                return Result.Failure<AuthResult>(updateRt.Error!);

            // Publish integration event (fire-and-forget by interface contract)
            var @event = new UserRegisteredEvent(int.Parse(userResult.Value.Id), userResult.Value.Email);
            var options = new PublishOptions
            {
                ExchangeName = "user_exchange",
                TypeExchange = ExchangeType.Topic,
                RoutingKey = "user.registered",
                Durable = true,
            };

            _eventBus.PublishAsync(@event, options);

            return Result.Success(new AuthResult(accessTokenResult.Value!, refreshTokenResult.Value!, userResult.Value));
        }
    }
}
