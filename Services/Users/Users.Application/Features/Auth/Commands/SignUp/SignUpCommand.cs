using BuildingBlocks.CQRS.Requests.Commands;
using Users.Application.DTOs.Auth;

namespace Users.Application.Features.Auth.Commands.SignUp
{
    public record SignUpCommand(
        string FirstName,
        string LastName,
        string Email,
        string Password)
        : ICommand<AuthResult>;
}
