using BuildingBlocks.Results;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Users.Application.Contracts;

namespace Users.Infrastructure.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
        private const string Scopes = "openid email profile";
        private const int DefaultClockToleranceSeconds = 300;

        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<GoogleAuthService> logger)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public Result<string> GetLoginUrl(string? state)
        {
            var clientId = _configuration["GoogleAuth:ClientId"];
            var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(redirectUri))
            {
                _logger.LogError(
                    "[GoogleAuth] Configuracao ausente. ClientId presente={HasClientId} RedirectUri presente={HasRedirectUri}",
                    !string.IsNullOrWhiteSpace(clientId),
                    !string.IsNullOrWhiteSpace(redirectUri));

                return Result<string>.Failure(Error.Problem("Google Auth configuration is missing."));
            }

            // Todos os valores precisam ser percent-encoded: o resultado vira o header
            // Location de um 302, e espacos crus (ex.: no scope) geram um header invalido.
            var query = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["scope"] = Scopes,
                ["access_type"] = "offline",
                ["include_granted_scopes"] = "true",
                ["prompt"] = "select_account",
                ["state"] = state ?? string.Empty
            };

            var url = $"{AuthorizationEndpoint}?{BuildQueryString(query)}";

            _logger.LogInformation(
                "[GoogleAuth] URL de login gerada. RedirectUri={RedirectUri} State={State}",
                redirectUri,
                string.IsNullOrWhiteSpace(state) ? "-" : state);

            return Result<string>.Success(url);
        }

        public async Task<Result<string>> ExchangeCodeForIdTokenAsync(string code, CancellationToken cancellationToken = default)
        {
            var clientId = _configuration["GoogleAuth:ClientId"];
            var clientSecret = _configuration["GoogleAuth:ClientSecret"];
            var redirectUri = _configuration["GoogleAuth:RedirectUri"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(redirectUri))
            {
                _logger.LogError(
                    "[GoogleAuth] Configuracao ausente na troca de codigo. ClientId={HasClientId} ClientSecret={HasClientSecret} RedirectUri={HasRedirectUri}",
                    !string.IsNullOrWhiteSpace(clientId),
                    !string.IsNullOrWhiteSpace(clientSecret),
                    !string.IsNullOrWhiteSpace(redirectUri));

                return Result<string>.Failure(Error.Problem("Google Auth configuration is missing."));
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();

                // O redirect_uri aqui precisa ser identico ao usado em GetLoginUrl,
                // senao o Google responde redirect_uri_mismatch.
                var tokenResponse = await httpClient.PostAsync(TokenEndpoint,
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
                {
                    var googleError = TryExtractGoogleError(tokenJson);

                    _logger.LogError(
                        "[GoogleAuth] Falha na troca de codigo. Status={Status} RedirectUri={RedirectUri} Erro={Error}",
                        (int)tokenResponse.StatusCode,
                        redirectUri,
                        googleError ?? tokenJson);

                    return Result<string>.Failure(Error.Problem(
                        $"Failed to exchange authorization code for token: {googleError ?? tokenResponse.StatusCode.ToString()}"));
                }

                using var tokenDoc = JsonDocument.Parse(tokenJson);

                if (!tokenDoc.RootElement.TryGetProperty("id_token", out var idTokenElement))
                {
                    _logger.LogError("[GoogleAuth] Resposta do Google nao contem id_token.");
                    return Result<string>.Failure(Error.Problem("No id_token received from Google."));
                }

                var idToken = idTokenElement.GetString();

                if (string.IsNullOrWhiteSpace(idToken))
                    return Result<string>.Failure(Error.Problem("No id_token received from Google."));

                return Result<string>.Success(idToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GoogleAuth] Excecao na troca de codigo por token.");
                return Result<string>.Failure(Error.Problem($"Token exchange failed: {ex.Message}"));
            }
        }

        public async Task<Result<GoogleJsonWebSignature.Payload>> ValidateIdTokenAsync(string idToken)
        {
            var audiences = new[]
            {
                _configuration["GoogleAuth:ClientId"],
                _configuration["GoogleAuth:ClientIdAndroid"],
                _configuration["GoogleAuth:ClientIdIOS"]
            }.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

            if (audiences.Count == 0)
            {
                _logger.LogError("[GoogleAuth] Nenhum ClientId configurado para validar o id_token.");
                return Result<GoogleJsonWebSignature.Payload>.Failure(Error.Problem("Google Auth configuration is missing."));
            }

            // Sem tolerancia, alguns segundos de diferenca entre o relogio do servidor e o
            // do Google fazem o id_token ser recusado com "JWT is not yet valid" (iat no
            // futuro) ou "expired". Isso e comum em container/VM cujo relogio deriva.
            var tolerance = TimeSpan.FromSeconds(
                int.TryParse(_configuration["GoogleAuth:ClockToleranceSeconds"], out var configured)
                    ? configured
                    : DefaultClockToleranceSeconds);

            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = audiences,
                    IssuedAtClockTolerance = tolerance,
                    ExpirationTimeClockTolerance = tolerance
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return Result<GoogleJsonWebSignature.Payload>.Success(payload);
            }
            catch (InvalidJwtException ex)
            {
                // Desvio de relogio maior que a tolerancia e a causa mais comum aqui,
                // entao logamos o horario do servidor para tornar isso obvio.
                _logger.LogWarning(
                    ex,
                    "[GoogleAuth] id_token invalido. HoraServidorUtc={ServerUtc} ToleranciaSegundos={Tolerance}",
                    DateTime.UtcNow.ToString("O"),
                    tolerance.TotalSeconds);

                return Result<GoogleJsonWebSignature.Payload>.Failure(Error.Problem("Token do Google invalido."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GoogleAuth] Falha inesperada ao validar o id_token.");
                return Result<GoogleJsonWebSignature.Payload>.Failure(Error.Problem("Falha ao validar o token do Google."));
            }
        }

        private static string BuildQueryString(Dictionary<string, string> parameters) =>
            string.Join("&", parameters.Select(p =>
                $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

        private static string? TryExtractGoogleError(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                var error = root.TryGetProperty("error", out var errorElement)
                    ? errorElement.GetString()
                    : null;

                var errorDescription = root.TryGetProperty("error_description", out var descriptionElement)
                    ? descriptionElement.GetString()
                    : null;

                if (error is null && errorDescription is null)
                    return null;

                return errorDescription is null ? error : $"{error}: {errorDescription}";
            }
            catch
            {
                return null;
            }
        }
    }
}
