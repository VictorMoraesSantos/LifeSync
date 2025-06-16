using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Messaging.Abstractions;
using BuildingBlocks.Messaging.Options;
using RabbitMQ.Client;
using Users.Application.DTOs.Auth;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;
using Users.Domain.Entities;
using Users.Domain.Events;

namespace Users.Application.Features.Auth.Commands.SignUp
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, AuthResponse>
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

        public async Task<AuthResponse> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            UserDTO dto = await _authService.SignUpAsync(
                command.FirstName,
                command.LastName,
                command.Email,
                command.Password);

            string accessToken = _tokenGenerator.GenerateTokenAsync(
                dto.Id,
                dto.Email,
                dto.Roles,
                cancellationToken);

            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            await _authService.UpdateRefreshTokenAsync(dto.Id, refreshToken);

            var @event = new UserRegisteredEvent(int.Parse(dto.Id), dto.Email);

            var options = new PublishOptions
            {
                ExchangeName = "user_exchange",
                TypeExchange = ExchangeType.Topic,
                RoutingKey = "user.registered",
                Durable = true,
            };

            _eventBus.PublishAsync(@event, options);

            return new AuthResponse(accessToken, refreshToken, dto);
        }
    }
}
