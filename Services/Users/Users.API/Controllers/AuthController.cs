using BuildingBlocks.CQRS.Sender;
using BuildingBlocks.Results;
using Core.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Application.Features.Auth.Commands.ChangePassword;
using Users.Application.Features.Auth.Commands.ForgotPassword;
using Users.Application.Features.Auth.Commands.GoogleCallback;
using Users.Application.Features.Auth.Commands.Login;
using Users.Application.Features.Auth.Commands.Logout;
using Users.Application.Features.Auth.Commands.ResetPassword;
using Users.Application.Features.Auth.Commands.SendEmailConfirmation;
using Users.Application.Features.Auth.Commands.ExternalLogin;
using Users.Application.Features.Auth.Queries.GetGoogleLoginUrl;
using Users.Application.DTOs.Auth;
using Users.Application.Features.Auth.Commands.SignUp;

namespace Users.API.Controllers
{
    public class AuthController : ApiController
    {
        private readonly ISender _sender;
        private readonly IConfiguration _configuration;

        public AuthController(ISender sender, IConfiguration configuration)
        {
            _sender = sender;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<HttpResult<object>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<HttpResult<object>> Register([FromBody] SignUpCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [AllowAnonymous]
        [HttpPost("external-login")]
        public async Task<HttpResult<object>> ExternalLogin([FromBody] ExternalLoginCommand command, CancellationToken cancellationToken)
        {
            var result = await _sender.Send(command, cancellationToken);

            return result.IsSuccess
                ? HttpResult<object>.Ok(result.Value!)
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }

        [AllowAnonymous]
        [HttpGet("google-login")]
        public async Task<IActionResult> GoogleLogin([FromQuery] string state, CancellationToken cancellationToken)
        {
            var query = new GetGoogleLoginUrlQuery(state);
            var result = await _sender.Send(query, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result.Error!.Description);

            return Redirect(result.Value!.Url);
        }

        [AllowAnonymous]
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string code, [FromQuery] string state, CancellationToken cancellationToken)
        {
            var appScheme = _configuration["GoogleAuth:AppScheme"] ?? "com.lifesync.app";

            var command = new GoogleCallbackCommand(code, state);
            var result = await _sender.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return Redirect($"{appScheme}://callback?error={Uri.EscapeDataString(result.Error!.Description)}");

            var authResult = result.Value!;
            var callbackUrl =
                $"{appScheme}://callback" +
                $"?access_token={Uri.EscapeDataString(authResult.AccessToken)}" +
                $"&refresh_token={Uri.EscapeDataString(authResult.RefreshToken)}" +
                $"&user_id={Uri.EscapeDataString(authResult.User.Id)}" +
                $"&state={Uri.EscapeDataString(state ?? "")}";

            return Redirect(callbackUrl);
        }

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

        [HttpPost("change-password")]
        public async Task<HttpResult<object>> ChangePassword([FromBody] ChangePasswordDTO request, CancellationToken cancellationToken)
        {
            var command = new ChangePasswordCommand(User, request.CurrentPassword, request.NewPassword);
            var result = await _sender.Send(command, cancellationToken);
            return result.IsSuccess
                ? HttpResult<object>.Updated()
                : HttpResult<object>.BadRequest(result.Error!.Description);
        }
    }
}
