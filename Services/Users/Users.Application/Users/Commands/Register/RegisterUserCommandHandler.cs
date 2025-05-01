using MediatR;
using Users.Application.DTOs.Auth;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;

namespace Users.Application.Users.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenGenerator _tokenGenerator;

        public RegisterUserCommandHandler(IIdentityService identityService, ITokenGenerator tokenGenerator)
        {
            _identityService = identityService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponse> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            // Cria o usuário
            var userId = await _identityService.CreateUserAsync(
                command.UserName,
                command.Password,
                command.Email,
                command.FirstName,
                command.LastName,
                command.Roles);

            // Busca detalhes do usuário
            var userDetails = await _identityService.GetUserDetailsAsync(userId);

            // Gera tokens
            var accessToken = _tokenGenerator.GenerateTokenAsync(
                userId,
                userDetails.Email,
                userDetails.Roles,
                cancellationToken);

            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            // Atualiza o refresh token no usuário
            await _identityService.UpdateUserRefreshTokenAsync(
                userId,
                refreshToken,
                DateTime.UtcNow.AddDays(7) // ou use o valor do seu JwtSettings
            );

            // Monta resposta
            var userSummary = new UserSummaryDTO
            {
                Id = userDetails.Id,
                UserName = userDetails.UserName,
                Email = userDetails.Email
            };

            return new AuthResponse(accessToken, refreshToken, userSummary);
        }
    }
}
