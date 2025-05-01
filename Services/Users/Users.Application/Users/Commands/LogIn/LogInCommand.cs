using MediatR;
using Users.Application.DTOs.Auth;

namespace Users.Application.Users.Commands.LogIn
{
    public record LogInCommand(string Email, string Password)
        : IRequest<AuthResponse>;
}
