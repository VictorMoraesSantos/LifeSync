using BuildingBlocks.Results;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Users.Application.Contracts;

namespace Users.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;

        public GoogleAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Result<GoogleJsonWebSignature.Payload>> ValidateIdTokenAsync(string idToken)
        {
            try
            {
                var audiences = new[]
                {
                    _configuration["GoogleAuth:ClientId"],
                    _configuration["GoogleAuth:ClientIdAndroid"],
                    _configuration["GoogleAuth:ClientIdIOS"]
                }.Where(a => !string.IsNullOrEmpty(a)).ToList();

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = audiences
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return Result<GoogleJsonWebSignature.Payload>.Success(payload);
            }
            catch (InvalidJwtException)
            {
                return Result<GoogleJsonWebSignature.Payload>.Failure(Error.Problem("Token do Google invalido."));
            }
        }
    }
}
