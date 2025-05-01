using BuildingBlocks.Exceptions;
using MediatR;
using Users.Application.DTOs.Auth;
using Users.Application.DTOs.User;
using Users.Application.Interfaces;

namespace Users.Application.Users.Commands.LogIn
{
    public class LogInCommandHandler : IRequestHandler<LogInCommand, AuthResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenGenerator _tokenGenerator;

        public LogInCommandHandler(IIdentityService identityService, ITokenGenerator tokenGenerator)
        {
            _identityService = identityService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResponse> Handle(LogInCommand command, CancellationToken cancellationToken)
        {
            bool signResult = await _identityService.SignInUserAsync(command.Email, command.Password);
            if (!signResult)
                throw new BadRequestException("Invalid credentials");

            string userId = await _identityService.GetUserIdAsync(command.Email);

            UserDetailsDTO userDetails = await _identityService.GetUserDetailsAsync(userId);

            string accessToken = _tokenGenerator.GenerateTokenAsync(
                userId,
                userDetails.Email,
                userDetails.Roles,
                cancellationToken);

            string refreshToken = _tokenGenerator.GenerateRefreshToken();

            await _identityService.UpdateUserRefreshTokenAsync(
                userId,
                refreshToken,
                DateTime.UtcNow.AddDays(7) // ou use o valor do seu JwtSettings
            );

            var userSummary = new UserSummaryDTO
            {
                Id = userDetails.Id,
                FullName = userDetails.FullName,
                UserName = userDetails.UserName,
                Email = userDetails.Email
            };

            return new AuthResponse(accessToken, refreshToken, userSummary);
        }
    }
}
