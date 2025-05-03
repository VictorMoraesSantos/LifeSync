using MediatR;
using Users.Application.DTOs.Auth;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;

namespace Users.Application.Users.Commands.Register
{
    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, AuthResponse>
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly ITokenGenerator _tokenGenerator;

        public SignUpCommandHandler(IAuthService identityService, ITokenGenerator tokenGenerator, IUserService userService)
        {
            _authService = identityService;
            _tokenGenerator = tokenGenerator;
            _userService = userService;
        }

        public async Task<AuthResponse> Handle(SignUpCommand command, CancellationToken cancellationToken)
        {
            UserDTO userDTO = await _authService.SignUpAsync(
                command.FirstName,
                command.LastName,
                command.Email,
                command.Password);

            string accessToken = _tokenGenerator.GenerateTokenAsync(
                userDTO.Id,
                userDTO.Email,
                userDTO.Roles,
                cancellationToken);

            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            await _authService.UpdateRefreshTokenAsync(userDTO.Id, refreshToken);

            return new AuthResponse(accessToken, refreshToken, userDTO);
        }
    }
}
