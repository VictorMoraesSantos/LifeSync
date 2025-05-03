using MediatR;

namespace Users.Application.Users.Commands.ChangePassword
{
    public record ChangePassword(string CurrentPassword, string NewPassword);
}

