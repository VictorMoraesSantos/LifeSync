# Autenticacao com Google - LifeSync

## Visao Geral

Autenticacao OAuth 2.0 com Google implementada usando o fluxo **Authorization Code via backend**. O app MAUI nao se comunica diretamente com o Google para obter tokens - o backend atua como intermediario, garantindo que o Client Secret nunca seja exposto no cliente.

---

## Arquitetura do Fluxo

```
MAUI App                     YARP Gateway                  Users.API                    Google
────────                     ────────────                  ─────────                    ──────

1. Toca "Continuar com Google"
2. WebAuthenticator abre
   browser para:
   /auth/google-login
                         3. Roteia para
                            /api/auth/google-login
                                                      4. Monta URL do Google
                                                         OAuth com client_id,
                                                         redirect_uri, scope
                                                      5. Retorna Redirect 302
                                                                              6. Exibe tela de
                                                                                 consentimento
                                                                              7. Usuario autoriza
                                                                              8. Redireciona para
                                                                                 /auth/google-callback
                                                                                 com ?code=AUTH_CODE
                         9. Roteia para
                            /api/auth/google-callback
                                                     10. Troca code por tokens
                                                         via POST para
                                                         googleapis.com/token
                                                     11. Extrai id_token da
                                                         resposta
                                                     12. Valida id_token com
                                                         GoogleJsonWebSignature
                                                     13. Busca/Cria usuario
                                                         no Identity
                                                     14. Gera JWT + RefreshToken
                                                     15. Redireciona para:
                                                         com.lifesync.app://callback
                                                         ?access_token=JWT
                                                         &refresh_token=REFRESH
                                                         &user_id=ID

16. WebAuthenticator
    captura o redirect
17. Extrai tokens dos
    query parameters
18. Armazena no
    SecureStorage
19. Inicializa UserSession
20. Navega para MainPage
```

---

## Configuracao no Google Cloud Console

1. Acessar [console.cloud.google.com](https://console.cloud.google.com)
2. Criar/selecionar projeto
3. Em **APIs & Services > Credentials**, criar **OAuth 2.0 Client ID** do tipo **Web Application**
4. Configurar:
   - **Authorized JavaScript origins**: `https://api.lifesync.tech`
   - **Authorized redirect URIs**: `https://api.lifesync.tech/auth/google-callback`
5. Em **OAuth Consent Screen**, configurar escopos: `email`, `profile`, `openid`
6. Anotar o **Client ID** e o **Client Secret**

> **Importante**: O Client Secret e usado apenas no backend (server-side). Nunca expor no app mobile.

---

## Backend (Users.API)

### Configuracao

#### NuGet Packages

**Users.Infrastructure.csproj**:
```xml
<PackageReference Include="Google.Apis.Auth" Version="1.73.0" />
```

**Users.Application.csproj**:
```xml
<PackageReference Include="Google.Apis.Auth" Version="1.73.0" />
```

#### appsettings.json

```json
{
  "GoogleAuth": {
    "ClientId": "",
    "ClientSecret": "",
    "RedirectUri": "https://api.lifesync.tech/auth/google-callback",
    "AppScheme": "com.lifesync.app"
  }
}
```

> **ClientId** e **ClientSecret** sao injetados via variaveis de ambiente em producao (nunca commitados no repositorio).

#### Variaveis de Ambiente (Docker)

```yaml
# docker-compose.override.yml / docker-compose.apps.yml
environment:
  - GoogleAuth__ClientId=${GOOGLE_CLIENT_ID}
  - GoogleAuth__ClientSecret=${GOOGLE_CLIENT_SECRET}
```

Arquivo `.env` (ignorado pelo git):
```
GOOGLE_CLIENT_ID=SEU_CLIENT_ID.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=GOCSPX-xxxxxxxxxxxxxxxx
```

---

### Arquivos e Implementacao

#### 1. Contrato - IGoogleAuthService

**Arquivo**: `Services/Users/Users.Application/Contracts/IGoogleAuthService.cs`

```csharp
using BuildingBlocks.Results;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace Users.Application.Contracts
{
    public interface IGoogleAuthService
    {
        Task<Result<Payload>> ValidateIdTokenAsync(string idToken);
    }
}
```

Recebe um ID Token do Google e retorna o Payload validado contendo dados do usuario (email, nome, subject ID).

---

#### 2. Implementacao - GoogleAuthService

**Arquivo**: `Services/Users/Users.Infrastructure/Services/GoogleAuthService.cs`

```csharp
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
                return Result<GoogleJsonWebSignature.Payload>.Failure(
                    Error.Problem("Token do Google invalido."));
            }
        }
    }
}
```

- Valida o ID Token usando `GoogleJsonWebSignature.ValidateAsync`
- Aceita multiplos Client IDs como audiences validas (Web, Android, iOS)
- Filtra audiences vazias para suportar configuracoes parciais

---

#### 3. DTO - ExternalLoginRequest

**Arquivo**: `Services/Users/Users.Application/DTOs/Auth/ExternalLoginRequest.cs`

```csharp
namespace Users.Application.DTOs.Auth
{
    public record ExternalLoginRequest(string IdToken, string Provider);
}
```

---

#### 4. Command CQRS - ExternalLoginCommand

**Arquivo**: `Services/Users/Users.Application/Features/Auth/Commands/ExternalLogin/ExternalLoginCommand.cs`

```csharp
using BuildingBlocks.CQRS.Requests.Commands;
using Users.Application.DTOs.Auth;

namespace Users.Application.Features.Auth.Commands.ExternalLogin
{
    public record ExternalLoginCommand(string idToken, string Provider) : ICommand<AuthResult>;
}
```

---

#### 5. Handler CQRS - ExternalLoginCommandHandler

**Arquivo**: `Services/Users/Users.Application/Features/Auth/Commands/ExternalLogin/ExternalLoginCommandHandler.cs`

```csharp
using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Users.Application.Contracts;
using Users.Application.DTOs.Auth;
using Users.Application.Interfaces;

namespace Users.Application.Features.Auth.Commands.ExternalLogin
{
    public class ExternalLoginCommandHandler : ICommandHandler<ExternalLoginCommand, AuthResult>
    {
        private readonly IGoogleAuthService _googleAuthService;
        private readonly IAuthService _authService;
        private readonly ITokenGenerator _tokenGenerator;

        public ExternalLoginCommandHandler(
            IGoogleAuthService googleAuthService,
            IAuthService authService,
            ITokenGenerator tokenGenerator)
        {
            _googleAuthService = googleAuthService;
            _authService = authService;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Result<AuthResult>> Handle(
            ExternalLoginCommand command, CancellationToken cancellationToken)
        {
            // 1. Validar ID Token do Google
            var payload = await _googleAuthService.ValidateIdTokenAsync(command.idToken);
            if (!payload.IsSuccess) return Result<AuthResult>.Failure(payload.Error);

            // 2. Extrair dados do usuario do payload
            var firstName = payload.Value?.GivenName ?? payload.Value?.Name ?? "User";
            var lastName = payload.Value?.FamilyName ?? "";

            // 3. Buscar ou criar usuario no Identity
            var userResult = await _authService.ExternalLoginAsync(
                payload.Value?.Email, firstName, lastName, "Google", payload.Value?.Subject);
            if (!userResult.IsSuccess)
                return Result<AuthResult>.Failure(userResult.Error);

            // 4. Gerar JWT Access Token
            var accessTokenResult = _tokenGenerator.GenerateToken(
                userResult.Value!.Id,
                userResult.Value.Email,
                userResult.Value.Roles,
                cancellationToken);
            if (!accessTokenResult.IsSuccess)
                return Result.Failure<AuthResult>(accessTokenResult.Error!);

            // 5. Gerar Refresh Token
            var refreshTokenResult = _tokenGenerator.GenerateRefreshToken();
            if (!refreshTokenResult.IsSuccess)
                return Result.Failure<AuthResult>(refreshTokenResult.Error!);

            // 6. Persistir Refresh Token no usuario
            await _authService.UpdateRefreshTokenAsync(
                userResult.Value.Id, refreshTokenResult.Value!);

            return Result.Success(new AuthResult(
                accessTokenResult.Value!,
                refreshTokenResult.Value!,
                userResult.Value));
        }
    }
}
```

Fluxo do handler:
1. Valida o ID Token do Google via `IGoogleAuthService`
2. Extrai `GivenName`, `FamilyName`, `Email` e `Subject` do payload
3. Busca usuario por email ou cria um novo (sem senha) via `IAuthService.ExternalLoginAsync`
4. Vincula o login externo ao usuario via `UserLoginInfo` do Identity
5. Gera JWT Access Token e Refresh Token
6. Retorna `AuthResult` com tokens e dados do usuario

---

#### 6. AuthService.ExternalLoginAsync

**Arquivo**: `Services/Users/Users.Infrastructure/Services/AuthService.cs` (metodo relevante)

```csharp
public async Task<Result<UserDTO>> ExternalLoginAsync(
    string email, string firstname, string lastname,
    string provider, string providerKey)
{
    try
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            // Criar novo usuario sem senha (login externo)
            var name = new Name(firstname, lastname);
            var contact = new Contact(email);
            user = new User(name, contact);
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                return Result<UserDTO>.Failure(Error.Problem(
                    string.Join("; ", createResult.Errors.Select(e => e.Description))));

            await _userManager.AddToRoleAsync(user, "User");
        }

        // Vincular login externo se ainda nao existir
        var existingLogins = await _userManager.GetLoginsAsync(user);
        if (!existingLogins.Any(l => l.LoginProvider == provider))
        {
            var loginInfo = new UserLoginInfo(provider, providerKey, provider);
            await _userManager.AddLoginAsync(user, loginInfo);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var dto = UserMapper.ToDto(user) with { Roles = roles.ToList() };

        return Result<UserDTO>.Success(dto);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error while logging in user {Email} with provider {Provider}",
            email, provider);
        return Result<UserDTO>.Failure(Error.Failure(ex.Message));
    }
}
```

- Se o usuario nao existe, cria sem senha (usuarios Google nao tem senha local)
- Vincula o `providerKey` (Google Subject ID) via `UserLoginInfo` do ASP.NET Identity
- Se o usuario ja existe (mesmo email), apenas vincula o login externo

---

#### 7. Endpoints no AuthController

**Arquivo**: `Services/Users/Users.API/Controllers/AuthController.cs`

Tres endpoints relacionados ao Google Auth:

##### POST /api/auth/external-login

Endpoint generico para login externo via ID Token (uso direto, sem fluxo OAuth via browser).

```csharp
[AllowAnonymous]
[HttpPost("external-login")]
public async Task<HttpResult<object>> ExternalLogin(
    [FromBody] ExternalLoginCommand command, CancellationToken cancellationToken)
{
    var result = await _sender.Send(command, cancellationToken);

    return result.IsSuccess
        ? HttpResult<object>.Ok(result.Value!)
        : HttpResult<object>.BadRequest(result.Error!.Description);
}
```

##### GET /api/auth/google-login

Inicia o fluxo OAuth redirecionando para o Google. Chamado pelo `WebAuthenticator` do MAUI.

```csharp
[AllowAnonymous]
[HttpGet("google-login")]
public IActionResult GoogleLogin([FromQuery] string state)
{
    var clientId = _configuration["GoogleAuth:ClientId"];
    var redirectUri = _configuration["GoogleAuth:RedirectUri"];

    var googleUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
        $"?client_id={clientId}" +
        $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
        "&response_type=code" +
        "&scope=openid email profile" +
        "&access_type=offline" +
        $"&state={Uri.EscapeDataString(state ?? "")}";

    return Redirect(googleUrl);
}
```

Parametros da URL do Google:
- `client_id`: Client ID da aplicacao Web
- `redirect_uri`: URL do callback no backend
- `response_type=code`: Fluxo Authorization Code
- `scope`: Permissoes solicitadas (openid, email, profile)
- `access_type=offline`: Solicita refresh token do Google
- `state`: Token anti-CSRF gerado pelo app MAUI

##### GET /api/auth/google-callback

Recebe o authorization code do Google, troca por tokens, e redireciona para o app.

```csharp
[AllowAnonymous]
[HttpGet("google-callback")]
public async Task<IActionResult> GoogleCallback(
    [FromQuery] string code, [FromQuery] string state, CancellationToken cancellationToken)
{
    var appScheme = _configuration["GoogleAuth:AppScheme"] ?? "com.lifesync.app";

    if (string.IsNullOrEmpty(code))
        return Redirect($"{appScheme}://callback?error=no_code");

    try
    {
        // 1. Trocar authorization code por tokens
        var clientId = _configuration["GoogleAuth:ClientId"];
        var clientSecret = _configuration["GoogleAuth:ClientSecret"];
        var redirectUri = _configuration["GoogleAuth:RedirectUri"];

        using var httpClient = new HttpClient();
        var tokenResponse = await httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = clientId!,
                ["client_secret"] = clientSecret!,
                ["redirect_uri"] = redirectUri!,
                ["grant_type"] = "authorization_code"
            }), cancellationToken);

        var tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

        if (!tokenResponse.IsSuccessStatusCode)
            return Redirect($"{appScheme}://callback?error=token_exchange_failed");

        // 2. Extrair id_token da resposta
        var tokenDoc = System.Text.Json.JsonDocument.Parse(tokenJson);
        var idToken = tokenDoc.RootElement.GetProperty("id_token").GetString();

        if (string.IsNullOrEmpty(idToken))
            return Redirect($"{appScheme}://callback?error=no_id_token");

        // 3. Processar via ExternalLoginCommand (valida token, busca/cria usuario, gera JWT)
        var command = new ExternalLoginCommand(idToken, "Google");
        var result = await _sender.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return Redirect($"{appScheme}://callback?error=" +
                Uri.EscapeDataString(result.Error!.Description));

        // 4. Redirecionar para o app com tokens nos query parameters
        var authResult = result.Value!;
        return Redirect(
            $"{appScheme}://callback" +
            $"?access_token={Uri.EscapeDataString(authResult.AccessToken)}" +
            $"&refresh_token={Uri.EscapeDataString(authResult.RefreshToken)}" +
            $"&user_id={Uri.EscapeDataString(authResult.User.Id)}" +
            $"&state={Uri.EscapeDataString(state ?? "")}");
    }
    catch (Exception ex)
    {
        return Redirect($"{appScheme}://callback?error=" +
            Uri.EscapeDataString(ex.Message));
    }
}
```

Fluxo do callback:
1. Recebe `code` e `state` do Google
2. Faz POST para `googleapis.com/token` trocando o code por tokens (usando Client Secret)
3. Extrai o `id_token` da resposta
4. Reutiliza o `ExternalLoginCommand` para validar e gerar JWT proprio
5. Redireciona para `com.lifesync.app://callback` com access_token, refresh_token e user_id
6. Em caso de erro, redireciona com `?error=mensagem`

---

#### 8. Registro no DI

**Arquivo**: `Services/Users/Users.Infrastructure/DependencyInjection.cs`

```csharp
using Users.Application.Contracts;

// Dentro de AddIdentityServices():
services.AddScoped<IGoogleAuthService, GoogleAuthService>();
```

---

#### 9. YARP Gateway

**Arquivo**: `Services/ApiGateways/YarpApiGateway/appsettings.Production.json`

A rota `auth-route` cobre todos os endpoints de autenticacao, incluindo os de Google:

```json
{
  "auth-route": {
    "ClusterId": "users-cluster",
    "Match": {
      "Path": "/auth/{**catch-all}"
    },
    "Transforms": [
      { "PathPattern": "/api/auth/{**catch-all}" }
    ]
  }
}
```

- `/auth/google-login` → `/api/auth/google-login`
- `/auth/google-callback` → `/api/auth/google-callback`
- `/auth/external-login` → `/api/auth/external-login`
- **Sem** `AuthorizationPolicy` - todos os endpoints de auth sao publicos

---

## MAUI App (ClientApp/LifeSyncApp)

### Arquivos e Implementacao

#### 1. Configuracao Android - WebAuthenticator Callback

**Arquivo**: `Platforms/Android/WebAuthenticationCallbackActivity.cs`

```csharp
using Android.App;
using Android.Content;
using Android.Content.PM;

namespace LifeSyncApp
{
    [Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.lifesync.app",
        DataHost = "callback")]
    public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
    {
    }
}
```

- Subclasse obrigatoria de `WebAuthenticatorCallbackActivity` para .NET MAUI
- O `IntentFilter` registra o scheme `com.lifesync.app` com host `callback`
- Quando o backend redireciona para `com.lifesync.app://callback?...`, o Android captura via esta activity
- `NoHistory = true`: nao aparece no historico de atividades
- `Exported = true`: necessario no Android 12+ para activities com intent-filters

> **Nota**: Nao e necessario declarar nada no AndroidManifest.xml - os atributos `[Activity]` e `[IntentFilter]` geram a configuracao automaticamente no build.

---

#### 2. DTO - ExternalLoginRequest (MAUI)

**Arquivo**: `DTOs/Auth/ExternalLoginRequest.cs`

```csharp
namespace LifeSyncApp.DTOs.Auth
{
    public class ExternalLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
    }
}
```

---

#### 3. Interface - IAuthService

**Arquivo**: `Services/Auth/IAuthService.cs`

Metodo adicionado:

```csharp
Task<AuthResult> GoogleLoginAsync();
```

---

#### 4. Implementacao - AuthService.GoogleLoginAsync

**Arquivo**: `Services/Auth/AuthService.cs`

```csharp
public async Task<AuthResult> GoogleLoginAsync()
{
    System.Diagnostics.Debug.WriteLine("[Auth] Starting Google login via backend OAuth flow");

    var baseAddress = _httpClient.BaseAddress?.ToString().TrimEnd('/');
    var state = Guid.NewGuid().ToString("N");

    // 1. Abrir browser via WebAuthenticator apontando para o backend
    var authResult = await WebAuthenticator.Default.AuthenticateAsync(
        new WebAuthenticatorOptions
        {
            Url = new Uri($"{baseAddress}/auth/google-login?state={state}"),
            CallbackUrl = new Uri("com.lifesync.app://callback")
        });

    // 2. Verificar se houve erro
    var error = authResult?.Get("error");
    if (!string.IsNullOrEmpty(error))
        throw new InvalidOperationException(
            $"Erro no login com Google: {Uri.UnescapeDataString(error)}");

    // 3. Extrair tokens da resposta
    var accessToken = authResult?.Get("access_token");
    var refreshToken = authResult?.Get("refresh_token");
    var userId = authResult?.Get("user_id");

    if (string.IsNullOrEmpty(accessToken) ||
        string.IsNullOrEmpty(refreshToken) ||
        string.IsNullOrEmpty(userId))
        throw new InvalidOperationException("Resposta incompleta do login com Google.");

    // 4. Armazenar no SecureStorage
    await SecureStorage.SetAsync(AccessTokenKey, accessToken);
    await SecureStorage.SetAsync(RefreshTokenKey, refreshToken);
    await SecureStorage.SetAsync(UserIdKey, userId);

    return new AuthResult
    {
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        User = new UserDTO { Id = userId }
    };
}
```

Detalhes:
- Usa `WebAuthenticator.Default.AuthenticateAsync` do MAUI
- `Url`: aponta para o endpoint do backend (nao para o Google diretamente)
- `CallbackUrl`: scheme customizado que o Android captura via `WebAuthenticationCallbackActivity`
- `state`: token anti-CSRF para validar que a resposta veio do fluxo que iniciamos
- Os tokens (access_token, refresh_token, user_id) vem como query parameters da URL de callback
- `authResult.Get("key")` extrai valores dos query parameters capturados pelo WebAuthenticator

---

#### 5. AuthDelegatingHandler

**Arquivo**: `Services/Auth/AuthDelegatingHandler.cs`

O endpoint `/auth/external-login` foi adicionado a lista de rotas que nao precisam de Bearer token:

```csharp
if (!path.Contains("/auth/login") &&
    !path.Contains("/auth/register") &&
    !path.Contains("/auth/external-login"))
{
    // Adicionar Bearer token...
}
```

> Os endpoints `google-login` e `google-callback` nao passam pelo `AuthDelegatingHandler` pois sao chamados via browser (WebAuthenticator), nao via HttpClient.

---

#### 6. LoginViewModel.GoogleLoginAsync

**Arquivo**: `ViewModels/Auth/LoginViewModel.cs`

```csharp
public async Task GoogleLoginAsync()
{
    if (IsBusy) return;
    IsBusy = true;
    HasError = false;

    try
    {
        await _authService.GoogleLoginAsync();
        await _userSession.InitializeAsync();

        await Shell.Current.GoToAsync("//MainPage");
    }
    catch (TaskCanceledException)
    {
        // Usuario cancelou o login (fechou o browser) - nao faz nada
    }
    catch (Exception ex)
    {
        HasError = true;
        ErrorMessage = ex.Message;
        System.Diagnostics.Debug.WriteLine($"Google Login Error: {ex}");
    }
    finally
    {
        IsBusy = false;
    }
}
```

- `TaskCanceledException`: capturada silenciosamente quando o usuario fecha o browser sem completar o login
- Apos sucesso: inicializa `UserSession` e navega para `MainPage`

---

#### 7. LoginPage.xaml.cs

**Arquivo**: `Views/Auth/LoginPage.xaml.cs`

```csharp
private async void OnGoogleLoginTapped(object sender, TappedEventArgs e)
{
    await _viewModel.GoogleLoginAsync();
}
```

---

## Resumo de Arquivos

### Backend

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `Users.Application/Contracts/IGoogleAuthService.cs` | Interface | Contrato de validacao do ID Token Google |
| `Users.Infrastructure/Services/GoogleAuthService.cs` | Servico | Valida ID Token via Google.Apis.Auth |
| `Users.Application/DTOs/Auth/ExternalLoginRequest.cs` | DTO | Request com IdToken e Provider |
| `Users.Application/Features/Auth/Commands/ExternalLogin/ExternalLoginCommand.cs` | Command | CQRS command com idToken e Provider |
| `Users.Application/Features/Auth/Commands/ExternalLogin/ExternalLoginCommandHandler.cs` | Handler | Orquestra validacao, criacao de usuario e geracao de JWT |
| `Users.Infrastructure/Services/AuthService.cs` | Servico | Metodo ExternalLoginAsync - busca/cria usuario via Identity |
| `Users.API/Controllers/AuthController.cs` | Controller | Endpoints: external-login, google-login, google-callback |
| `Users.Infrastructure/DependencyInjection.cs` | DI | Registro de IGoogleAuthService |
| `Users.Infrastructure/Users.Infrastructure.csproj` | Projeto | PackageReference Google.Apis.Auth |
| `Users.API/appsettings.json` | Config | Secao GoogleAuth (valores vazios - injetados via env) |

### MAUI App

| Arquivo | Tipo | Descricao |
|---------|------|-----------|
| `Platforms/Android/WebAuthenticationCallbackActivity.cs` | Activity | Captura callback do OAuth via IntentFilter |
| `DTOs/Auth/ExternalLoginRequest.cs` | DTO | Request com IdToken e Provider |
| `Services/Auth/IAuthService.cs` | Interface | Metodo GoogleLoginAsync |
| `Services/Auth/AuthService.cs` | Servico | Implementacao do fluxo OAuth via WebAuthenticator |
| `Services/Auth/AuthDelegatingHandler.cs` | Handler | Rota /auth/external-login sem token |
| `ViewModels/Auth/LoginViewModel.cs` | ViewModel | Metodo GoogleLoginAsync com tratamento de erros |
| `Views/Auth/LoginPage.xaml.cs` | View | Handler OnGoogleLoginTapped |

### Infraestrutura

| Arquivo | Descricao |
|---------|-----------|
| `docker-compose.override.yml` | Variaveis GoogleAuth__ClientId e GoogleAuth__ClientSecret |
| `deploy/docker-compose.apps.yml` | Variaveis via ${GOOGLE_CLIENT_ID} e ${GOOGLE_CLIENT_SECRET} |
| `.env` | Valores reais dos secrets (ignorado pelo git) |
| `.gitignore` | Ignora .env e deploy/.env |
| `YarpApiGateway/appsettings.Production.json` | Rota auth-route sem AuthorizationPolicy |

---

## Seguranca

- **Client Secret**: armazenado apenas no backend via variaveis de ambiente, nunca no app mobile ou no repositorio
- **ID Token**: validado server-side usando `Google.Apis.Auth` com audience restrita ao Client ID configurado
- **JWT**: tokens proprios do LifeSync gerados apos validacao do Google, armazenados no `SecureStorage` do MAUI (criptografado por plataforma)
- **State parameter**: token CSRF gerado pelo app para validar que a resposta veio do fluxo iniciado
- **Usuarios Google**: criados sem senha local, vinculados via `UserLoginInfo` do ASP.NET Identity
- **GitHub Push Protection**: secrets nao sao commitados - `.env` esta no `.gitignore`

---

## Comportamento do Usuario

- **Primeiro login**: Google exibe tela de consentimento solicitando permissao para email e perfil
- **Logins subsequentes**: Google reconhece a sessao ativa e redireciona automaticamente (tela branca rapida)
- **Cancelamento**: se o usuario fecha o browser, `TaskCanceledException` e capturada e nada acontece
- **Erro**: mensagem de erro exibida na tela de login do app
- **Usuario existente com mesmo email**: vincula o login Google ao usuario existente sem duplicar
