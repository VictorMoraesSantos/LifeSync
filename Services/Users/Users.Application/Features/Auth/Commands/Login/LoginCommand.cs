using BuildingBlocks.CQRS.Requests.Commands;
using Users.Application.DTOs.Auth;

namespace Users.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password) : ICommand<AuthResult>;
}
