using MediatR;
using Users.Application.DTOs.Auth;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;

namespace Users.Application.Auth.Commands.Login
{
    public class LogInCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;
        private readonly ITokenGenerator _tokenGenerator;

        public LogInCommandHandler(IAuthService identityService, ITokenGenerator tokenGenerator)
        {
            _authService = identityService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            UserDTO userDTO = await _authService.LoginAsync(command.Email, command.Password);

            string accessToken = _tokenGenerator.GenerateTokenAsync(
                userDTO.Id,
                userDTO.Email,
                userDTO.Roles,
                cancellationToken);

            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            // Aqui você pode atualizar o refresh token no banco via _authService ou outro serviço, se necessário
            // await _authService.UpdateUserRefreshTokenAsync(userDTO.Id, refreshToken, expirationDate);

            // Monta e retorna a resposta de autenticação
            return new AuthResponse(accessToken, refreshToken, userDTO);
        }
    }
}
