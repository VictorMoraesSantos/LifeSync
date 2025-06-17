using BuildingBlocks.CQRS.Sender;
using Core.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.DTOs.Auth;
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
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] SignUpCommand command)
        {
            var result = await _sender.Send(command);
            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> Logout()
        {
            LogoutCommand command = new LogoutCommand(User);
            await _sender.Send(command);
            return NoContent();
        }

        [HttpPost("send-email-confirmation")]
        public async Task<ActionResult> SendEmailConfirmation([FromBody] SendEmailConfirmationCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            await _sender.Send(command);
            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword request)
        {
            ChangePasswordCommand command = new ChangePasswordCommand(User, request.CurrentPassword, request.NewPassword);
            await _sender.Send(command);
            return NoContent();
        }
    }
}
