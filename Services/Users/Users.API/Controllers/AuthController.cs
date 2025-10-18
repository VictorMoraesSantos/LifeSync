using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Features.Auth.Commands.ChangePassword;
using Users.Application.Features.Auth.Commands.ForgotPassword;
using Users.Application.Features.Auth.Commands.Login;
using Users.Application.Features.Auth.Commands.Logout;
using Users.Application.Features.Auth.Commands.ResetPassword;
using Users.Application.Features.Auth.Commands.SendEmailConfirmation;
using Users.Application.Features.Auth.Commands.SignUp;

namespace Users.API.Controllers
{
    public class AuthController : ApiController
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost("login")]
        public async Task<HttpResult<object>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost("register")]
        public async Task<HttpResult<object>> Register([FromBody] SignUpCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<HttpResult<object>> Logout(CancellationToken cancellationToken)
        {
            var result = await _sender.Send(new LogoutCommand(User), cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost("send-email-confirmation")]
        public async Task<HttpResult<object>> SendEmailConfirmation([FromBody] SendEmailConfirmationCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost("forgot-password")]
        public async Task<HttpResult<object>> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [HttpPost("reset-password")]
        public async Task<HttpResult<object>> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<HttpResult<object>> ChangePassword([FromBody] ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var command = new ChangePasswordCommand(User, request.CurrentPassword, request.NewPassword);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }
    }
}
