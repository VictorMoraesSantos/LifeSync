using MediatR;
using Users.Application.DTOs.Auth;

namespace Users.Application.Auth.Commands.SignUp
{
    public record SignUpCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password)
        : IRequest<AuthResponse>;
}
