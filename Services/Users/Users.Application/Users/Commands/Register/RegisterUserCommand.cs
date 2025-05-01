using MediatR;
using Users.Application.DTOs.Auth;
using Users.Application.DTOs.User;

namespace Users.Application.Users.Commands.Register
{
    public record RegisterUserCommand(
        string UserName,
        string FirstName,
        string LastName,
        string Email,
        string Password,
        IList<string> Roles)
        : IRequest<AuthResponse>;
}
