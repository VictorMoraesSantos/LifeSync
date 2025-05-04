using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Auth.Commands.ChangePassword;
using Users.Application.Auth.Commands.ForgotPassword;
using Users.Application.Auth.Commands.Login;
using Users.Application.Auth.Commands.Logout;
using Users.Application.Auth.Commands.ResetPassword;
using Users.Application.Auth.Commands.SendEmailConfirmation;
using Users.Application.Auth.Commands.SignUp;
using Users.Application.DTOs.Auth;

namespace Users.API.Controllers
{
    public class AuthController : ApiController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] SignUpCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> Logout()
        {
            LogoutCommand command = new LogoutCommand(User);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("send-email-confirmation")]
        public async Task<ActionResult> SendEmailConfirmation([FromBody] SendEmailConfirmationCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassword request)
        {
            ChangePasswordCommand command = new ChangePasswordCommand(User, request.CurrentPassword, request.NewPassword);
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
