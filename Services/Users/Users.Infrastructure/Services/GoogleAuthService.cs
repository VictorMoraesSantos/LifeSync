using BuildingBlocks.Results;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Users.Application.Contracts;

namespace Users.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public GoogleAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public Result<string> GetLoginUrl(string? state)
        {
            var clientId = _configuration["GoogleAuth:ClientId"];
            var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(redirectUri))
                return Result<string>.Failure(Error.Problem("Google Auth configuration is missing."));

            var url = "https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id={clientId}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                "&response_type=code" +
                "&scope=openid email profile" +
                "&access_type=offline" +
                $"&state={Uri.EscapeDataString(state ?? "")}";

            return Result<string>.Success(url);
        }

        public async Task<Result<string>> ExchangeCodeForIdTokenAsync(string code, CancellationToken cancellationToken = default)
        {
            var clientId = _configuration["GoogleAuth:ClientId"];
            var clientSecret = _configuration["GoogleAuth:ClientSecret"];
            var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(redirectUri))
                return Result<string>.Failure(Error.Problem("Google Auth configuration is missing."));

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token",
                    new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        ["code"] = code,
                        ["client_id"] = clientId,
                        ["client_secret"] = clientSecret,
                        ["redirect_uri"] = redirectUri,
                        ["grant_type"] = "authorization_code"
                    }), cancellationToken);

                var tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

                if (!tokenResponse.IsSuccessStatusCode)
                    return Result<string>.Failure(Error.Problem("Failed to exchange authorization code for token."));

                var tokenDoc = JsonDocument.Parse(tokenJson);
                var idToken = tokenDoc.RootElement.GetProperty("id_token").GetString();

                if (string.IsNullOrEmpty(idToken))
                    return Result<string>.Failure(Error.Problem("No id_token received from Google."));

                return Result<string>.Success(idToken);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure(Error.Problem($"Token exchange failed: {ex.Message}"));
            }
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
