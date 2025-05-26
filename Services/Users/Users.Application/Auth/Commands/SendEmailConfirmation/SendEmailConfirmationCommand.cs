using MediatR;

namespace Users.Application.Auth.Commands.SendEmailConfirmation
{
    public record SendEmailConfirmationCommand(string Email) : IRequest;
}
