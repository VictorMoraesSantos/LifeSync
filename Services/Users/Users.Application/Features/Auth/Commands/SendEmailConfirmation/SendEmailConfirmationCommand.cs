using BuildingBlocks.CQRS.Commands;

namespace Users.Application.Features.Auth.Commands.SendEmailConfirmation
{
    public record SendEmailConfirmationCommand(string Email) : ICommand;
}
