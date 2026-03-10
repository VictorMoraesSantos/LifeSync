# Plano de Implementacao - Autenticacao com Google (LifeSync)

## Visao Geral

Este documento descreve o plano completo para implementar autenticacao com Google no LifeSync, cobrindo tanto o backend (Users.API) quanto o app mobile (MAUI).

---

## Fluxo de Autenticacao

```
MAUI App                              Backend (Users.API)
─────────                             ──────────────────
1. User toca "Login com Google"
2. WebAuthenticator abre browser
3. Google OAuth consent screen
4. Google retorna ID Token
5. Envia ID Token ao backend ────→    6. Recebe ID Token
                                      7. Valida com Google (Google.Apis.Auth)
                                      8. Busca/Cria usuario (Identity)
                                      9. Gera JWT + RefreshToken
                               ←────  10. Retorna AuthResult
11. Armazena tokens (SecureStorage)
12. Navega para MainPage
```

---

## Fase 1: Configuracao no Google Cloud Console

1. Acessar [console.cloud.google.com](https://console.cloud.google.com)
2. Criar um projeto ou selecionar existente
3. Ativar a **Google Identity API**
4. Em **Credentials**, criar:
   - **OAuth 2.0 Client ID (Web)** - para o backend validar tokens
   - **OAuth 2.0 Client ID (Android)** - com package name `com.lifesync.app` e SHA-1 fingerprint
   - **OAuth 2.0 Client ID (iOS)** - com bundle ID
5. Configurar a **OAuth Consent Screen** com os escopos: `email`, `profile`, `openid`

---

## Fase 2: Backend (Users.API)

### 2.1 - NuGet Package

Adicionar ao `Users.API.csproj`:

```xml
<PackageReference Include="Google.Apis.Auth" Version="1.68.0" />
```

### 2.2 - Configuracao no appsettings.json

**Arquivo:** `Services/Users/Users.API/appsettings.json`

```json
{
  "GoogleAuth": {
    "ClientId": "SEU_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
    "ClientIdAndroid": "SEU_GOOGLE_CLIENT_ID_ANDROID.apps.googleusercontent.com",
    "ClientIdIOS": "SEU_GOOGLE_CLIENT_ID_IOS.apps.googleusercontent.com"
  }
}
```

### 2.3 - DTO para Login Externo

**Criar:** `Services/Users/Users.Application/DTOs/ExternalLoginRequest.cs`

```csharp
namespace Users.Application.DTOs;

public record ExternalLoginRequest(
    string IdToken,
    string Provider // "Google"
);
```

### 2.4 - Servico de Validacao do Google Token

**Criar:** `Services/Users/Users.Infrastructure/Services/GoogleAuthService.cs`

```csharp
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Users.Infrastructure.Services;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload?> ValidateIdTokenAsync(string idToken);
}

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;

    public GoogleAuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<GoogleJsonWebSignature.Payload?> ValidateIdTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[]
                {
                    _configuration["GoogleAuth:ClientId"],
                    _configuration["GoogleAuth:ClientIdAndroid"],
                    _configuration["GoogleAuth:ClientIdIOS"]
                }
            };

            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}
```

### 2.5 - Novo Metodo no AuthService

**Editar:** `Services/Users/Users.Infrastructure/Services/AuthService.cs`

Adicionar a interface `IAuthService`:

```csharp
Task<Result<AuthResult>> ExternalLoginAsync(
    string email, string firstName, string lastName,
    string provider, string providerKey);
```

Implementacao:

```csharp
public async Task<Result<AuthResult>> ExternalLoginAsync(
    string email, string firstName, string lastName,
    string provider, string providerKey)
{
    var user = await _userManager.FindByEmailAsync(email);

    if (user is null)
    {
        // Criar novo usuario (sem senha - login externo)
        user = new User
        {
            Email = email,
            UserName = email,
            EmailConfirmed = true, // Google ja verificou o email
            Name = Name.Create(firstName, lastName),
            Contact = Contact.Create(email),
            IsActive = true
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToList();
            return Result<AuthResult>.Failure(errors);
        }

        await _userManager.AddToRoleAsync(user, "User");
    }

    // Vincular login externo se ainda nao existir
    var existingLogins = await _userManager.GetLoginsAsync(user);
    if (!existingLogins.Any(l => l.LoginProvider == provider))
    {
        var loginInfo = new UserLoginInfo(provider, providerKey, provider);
        await _userManager.AddLoginAsync(user, loginInfo);
    }

    // Gerar tokens JWT normalmente
    var roles = await _userManager.GetRolesAsync(user);
    var accessToken = _tokenGenerator.GenerateToken(user, roles);
    var refreshToken = _tokenGenerator.GenerateRefreshToken();

    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
    await _userManager.UpdateAsync(user);

    return Result<AuthResult>.Success(new AuthResult
    {
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        User = new UserDto
        {
            Id = user.Id.ToString(),
            FirstName = user.Name.FirstName,
            LastName = user.Name.LastName,
            FullName = user.Name.FullName,
            Email = user.Email!,
            IsActive = user.IsActive,
            Roles = roles.ToList()
        }
    });
}
```

### 2.6 - Command CQRS

**Criar:** `Services/Users/Users.Application/Commands/ExternalLogin/ExternalLoginCommand.cs`

```csharp
using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace Users.Application.Commands.ExternalLogin;

public record ExternalLoginCommand(string IdToken, string Provider)
    : ICommand<Result<AuthResult>>;
```

**Criar:** `Services/Users/Users.Application/Commands/ExternalLogin/ExternalLoginCommandHandler.cs`

```csharp
using BuildingBlocks.CQRS;
using BuildingBlocks.Results;

namespace Users.Application.Commands.ExternalLogin;

public class ExternalLoginCommandHandler
    : ICommandHandler<ExternalLoginCommand, Result<AuthResult>>
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IAuthService _authService;

    public ExternalLoginCommandHandler(
        IGoogleAuthService googleAuthService,
        IAuthService authService)
    {
        _googleAuthService = googleAuthService;
        _authService = authService;
    }

    public async Task<Result<AuthResult>> Handle(
        ExternalLoginCommand command, CancellationToken cancellationToken)
    {
        if (command.Provider != "Google")
            return Result<AuthResult>.Failure(["Provider nao suportado."]);

        var payload = await _googleAuthService.ValidateIdTokenAsync(command.IdToken);
        if (payload is null)
            return Result<AuthResult>.Failure(["Token do Google invalido."]);

        var firstName = payload.GivenName ?? payload.Name ?? "User";
        var lastName = payload.FamilyName ?? "";

        return await _authService.ExternalLoginAsync(
            payload.Email,
            firstName,
            lastName,
            "Google",
            payload.Subject // Google user ID
        );
    }
}
```

### 2.7 - Endpoint no AuthController

**Editar:** `Services/Users/Users.API/Controllers/AuthController.cs`

```csharp
[HttpPost("external-login")]
public async Task<IActionResult> ExternalLogin(
    [FromBody] ExternalLoginRequest request,
    CancellationToken cancellationToken)
{
    var command = new ExternalLoginCommand(request.IdToken, request.Provider);
    var result = await _mediator.Send(command, cancellationToken);

    if (!result.IsSuccess)
        return BadRequest(new { Success = false, Errors = result.Errors });

    return Ok(new { Success = true, Data = result.Value });
}
```

### 2.8 - Registrar no DI

**Editar:** `Services/Users/Users.Infrastructure/DependencyInjection.cs`

```csharp
services.AddScoped<IGoogleAuthService, GoogleAuthService>();
```

---

## Fase 3: MAUI App (ClientApp/LifeSyncApp)

### 3.1 - Configuracao Android

**Editar:** `Platforms/Android/AndroidManifest.xml` - adicionar dentro de `<application>`:

```xml
<activity android:name="Microsoft.Maui.Authentication.WebAuthenticatorCallbackActivity"
          android:exported="true">
    <intent-filter>
        <action android:name="android.intent.action.VIEW" />
        <category android:name="android.intent.category.DEFAULT" />
        <category android:name="android.intent.category.BROWSABLE" />
        <data android:scheme="com.lifesync.app" />
    </intent-filter>
</activity>
```

> Nota: Nao precisa de NuGet extra. O .NET MAUI ja tem `WebAuthenticator` built-in.

### 3.2 - DTO para External Login

**Criar:** `ClientApp/LifeSyncApp/DTOs/Auth/ExternalLoginRequest.cs`

```csharp
namespace LifeSyncApp.DTOs.Auth;

public class ExternalLoginRequest
{
    public string IdToken { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}
```

### 3.3 - Atualizar IAuthService

**Editar:** `ClientApp/LifeSyncApp/Services/Auth/IAuthService.cs`

Adicionar:

```csharp
Task<AuthResult> GoogleLoginAsync();
```

### 3.4 - Implementar no AuthService

**Editar:** `ClientApp/LifeSyncApp/Services/Auth/AuthService.cs`

```csharp
public async Task<AuthResult> GoogleLoginAsync()
{
    // 1. Abrir browser para login Google via WebAuthenticator
    var authResult = await WebAuthenticatorBridge.AuthenticateAsync(
        new Uri("https://accounts.google.com/o/oauth2/v2/auth?" +
            "client_id=SEU_GOOGLE_CLIENT_ID.apps.googleusercontent.com" +
            "&redirect_uri=com.lifesync.app:/" +
            "&response_type=id_token" +
            "&scope=openid email profile" +
            "&nonce=" + Guid.NewGuid().ToString()),
        new Uri("com.lifesync.app:/"));

    var idToken = authResult.IdToken;

    // 2. Enviar ao backend para validacao e criacao de sessao
    var client = _httpClientFactory.CreateClient("LifeSyncApi");
    var request = new ExternalLoginRequest
    {
        IdToken = idToken,
        Provider = "Google"
    };

    var response = await client.PostAsJsonAsync("/auth/external-login", request);
    var content = await response.Content.ReadAsStringAsync();

    if (!response.IsSuccessStatusCode)
    {
        var errorResponse = JsonSerializer.Deserialize<ApiSingleResponse<AuthResult>>(
            content, _jsonOptions);
        throw new Exception(
            errorResponse?.Errors?.FirstOrDefault() ?? "Erro no login com Google");
    }

    var apiResponse = JsonSerializer.Deserialize<ApiSingleResponse<AuthResult>>(
        content, _jsonOptions);

    if (apiResponse?.Success != true || apiResponse.Data is null)
        throw new Exception("Resposta invalida do servidor.");

    var result = apiResponse.Data;

    // 3. Armazenar tokens no SecureStorage
    await SecureStorage.Default.SetAsync(AccessTokenKey, result.AccessToken);
    await SecureStorage.Default.SetAsync(RefreshTokenKey, result.RefreshToken);
    await SecureStorage.Default.SetAsync(UserIdKey, result.User.Id);

    return result;
}
```

### 3.5 - Atualizar LoginViewModel

**Editar:** `ClientApp/LifeSyncApp/ViewModels/Auth/LoginViewModel.cs`

Adicionar metodo:

```csharp
public async Task GoogleLoginAsync()
{
    try
    {
        IsBusy = true;
        HasError = false;

        var result = await _authService.GoogleLoginAsync();

        await _userSession.InitializeAsync();

        await Shell.Current.GoToAsync("//MainPage");
    }
    catch (TaskCanceledException)
    {
        // Usuario cancelou o login - nao faz nada
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

### 3.6 - Atualizar LoginPage.xaml.cs

**Editar:** `ClientApp/LifeSyncApp/Views/Auth/LoginPage.xaml.cs`

Substituir o handler do botao Google (que atualmente mostra um alert):

```csharp
private async void OnGoogleLoginClicked(object sender, EventArgs e)
{
    if (BindingContext is LoginViewModel viewModel)
    {
        await viewModel.GoogleLoginAsync();
    }
}
```

### 3.7 - Atualizar AuthDelegatingHandler

**Editar:** `ClientApp/LifeSyncApp/Services/Auth/AuthDelegatingHandler.cs`

Adicionar `/auth/external-login` a lista de rotas que nao precisam de token:

```csharp
// Adicionar ao check de rotas sem auth
if (request.RequestUri?.AbsolutePath.Contains("/auth/login") == true ||
    request.RequestUri?.AbsolutePath.Contains("/auth/register") == true ||
    request.RequestUri?.AbsolutePath.Contains("/auth/external-login") == true)
{
    return await base.SendAsync(request, cancellationToken);
}
```

---

## Fase 4: Abordagem Alternativa - Google Sign-In Nativo (Opcional)

Para uma UX mais nativa (sem abrir browser), pode-se usar o Google Sign-In SDK diretamente no Android.

### Opcao A: Plugin.Firebase.Auth.Google

```xml
<PackageReference Include="Plugin.Firebase.Auth.Google" Version="3.0.0" />
```

### Opcao B: Google Sign-In SDK direto (Android)

**Criar:** `Platforms/Android/Services/GoogleSignInService.cs`

```csharp
using Android.Gms.Auth.Api.SignIn;

namespace LifeSyncApp.Platforms.Android.Services;

public class GoogleSignInService
{
    public static GoogleSignInClient GetGoogleSignInClient(Android.App.Activity activity)
    {
        var gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
            .RequestIdToken("SEU_GOOGLE_CLIENT_ID.apps.googleusercontent.com")
            .RequestEmail()
            .RequestProfile()
            .Build();

        return GoogleSignIn.GetClient(activity, gso);
    }
}
```

> **Recomendacao:** Comece com o `WebAuthenticator` (mais simples, cross-platform). Migre para SDK nativo depois se necessario.

---

## Resumo de Arquivos

### Criar

| Arquivo | Descricao |
|---------|-----------|
| `Users.Application/DTOs/ExternalLoginRequest.cs` | DTO do request |
| `Users.Application/Commands/ExternalLogin/ExternalLoginCommand.cs` | Command CQRS |
| `Users.Application/Commands/ExternalLogin/ExternalLoginCommandHandler.cs` | Handler CQRS |
| `Users.Infrastructure/Services/GoogleAuthService.cs` | Validacao do token Google |
| `ClientApp/LifeSyncApp/DTOs/Auth/ExternalLoginRequest.cs` | DTO no MAUI |

### Editar

| Arquivo | Alteracao |
|---------|-----------|
| `Users.API/Users.API.csproj` | NuGet Google.Apis.Auth |
| `Users.API/appsettings.json` | Secao GoogleAuth |
| `Users.API/Controllers/AuthController.cs` | Endpoint external-login |
| `Users.Infrastructure/Services/AuthService.cs` | Metodo ExternalLoginAsync |
| `Users.Infrastructure/DependencyInjection.cs` | Registrar GoogleAuthService |
| `ClientApp/LifeSyncApp/Services/Auth/IAuthService.cs` | Metodo GoogleLoginAsync |
| `ClientApp/LifeSyncApp/Services/Auth/AuthService.cs` | Implementacao GoogleLoginAsync |
| `ClientApp/LifeSyncApp/Services/Auth/AuthDelegatingHandler.cs` | Rota sem auth |
| `ClientApp/LifeSyncApp/ViewModels/Auth/LoginViewModel.cs` | Metodo GoogleLoginAsync |
| `ClientApp/LifeSyncApp/Views/Auth/LoginPage.xaml.cs` | Handler do botao |
| `Platforms/Android/AndroidManifest.xml` | Callback activity |

---

## Ordem de Implementacao

1. **Google Cloud Console** - criar credenciais OAuth (Web + Android + iOS)
2. **Backend: DTOs e Services** - ExternalLoginRequest, GoogleAuthService
3. **Backend: CQRS** - ExternalLoginCommand + Handler
4. **Backend: Controller + DI** - Endpoint e registro de servicos
5. **MAUI: DTO + AuthService** - ExternalLoginRequest, GoogleLoginAsync
6. **MAUI: ViewModel + View** - LoginViewModel, LoginPage handler
7. **MAUI: Android Config** - AndroidManifest.xml callback
8. **Testes** - Testar no Android Emulator primeiro, depois device fisico

---

## Consideracoes de Seguranca

- **Nunca** armazenar o Client Secret no app mobile
- O ID Token e validado no backend (server-side) usando `Google.Apis.Auth`
- Tokens JWT sao armazenados no `SecureStorage` (criptografado por plataforma)
- O endpoint `/auth/external-login` nao requer autenticacao previa
- Usuarios criados via Google tem `EmailConfirmed = true` automaticamente
- O `providerKey` (Google Subject ID) e vinculado via `UserLoginInfo` do Identity
