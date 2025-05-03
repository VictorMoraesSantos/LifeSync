using Core.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Users.Application.DTOs.Auth;
using Users.Application.Users.Commands.ChangePassword;
using Users.Application.Users.Commands.ForgotPassword;
using Users.Application.Users.Commands.LogIn;
using Users.Application.Users.Commands.LogOut;
using Users.Application.Users.Commands.Register;
using Users.Application.Users.Commands.ResendEmailConfirmation;
using Users.Application.Users.Commands.ResetPassword;

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
        public async Task<ActionResult<AuthResponse>> LogIn([FromBody] SignInCommand command)
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

        [HttpPost("logout")]
        public async Task<ActionResult> LogOut()
        {
            SignOutCommand command = new SignOutCommand(User);
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("resend-email-confirmation")]
        public async Task<ActionResult> ResendEmailConfirmation([FromBody] ResendEmailConfirmationCommand command)
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

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
