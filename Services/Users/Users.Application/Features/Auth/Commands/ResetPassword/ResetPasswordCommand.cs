using BuildingBlocks.CQRS.Commands;

namespace Users.Application.Features.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(string UserId, string Token, string NewPassword) : ICommand<ResetPasswordResult>;
    public record ResetPasswordResult(bool IsSuccess);
}
