using MediatR;
using Users.Application.DTOs.Auth;

namespace Users.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password)
        : IRequest<AuthResponse>;
}
