using BuildingBlocks.CQRS.Requests.Commands;
using BuildingBlocks.Results;
using Users.Application.DTOs.Auth;

namespace Users.Application.Features.Auth.Commands.ExternalLogin
{
    public record ExternalLoginCommand(string idToken, string Provider) : ICommand<AuthResult>;
}
