using BuildingBlocks.CQRS.Request;
using Users.Application.DTOs.Auth;

namespace Users.Application.Features.Auth.Commands.SignUp
{
    public record SignUpCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password)
        : IRequest<AuthResponse>;
}
