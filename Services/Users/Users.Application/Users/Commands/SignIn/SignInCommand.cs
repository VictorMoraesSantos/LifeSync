using MediatR;
using Users.Application.DTOs.Auth;

namespace Users.Application.Users.Commands.LogIn
{
    public record SignInCommand(string Email, string Password)
        : IRequest<AuthResponse>;
}
