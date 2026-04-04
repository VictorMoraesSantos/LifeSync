# Documentação do ClientApp (MAUI) - LifeSync

> **Data da análise:** 23 de março de 2026  
> **Versão do Projeto:** 1.0  
> **Versão do .NET:** net10.0  
> **Status:** Em desenvolvimento ativo

---

## Índice

1. [Visão Geral](#visão-geral)
2. [Estrutura do Projeto](#estrutura-do-projeto)
3. [Arquitetura](#arquitetura)
4. [Tecnologias e Dependências](#tecnologias-e-dependências)
5. [Problemas Críticos Identificados](#problemas-críticos-identificados)
6. [Recomendações de Correção](#recomendações-de-correção)
7. [Score/Qualidade do Frontend](#scorequalidade-do-frontend)
8. [Configuração e Ambiente](#configuração-e-ambiente)
9. [Navegação e Shell](#navegação-e-shell)
10. [Autenticação](#autenticação)
11. [Serviços](#serviços)
12. [Testes](#testes)

---

## Visão Geral

O **LifeSyncApp** é o cliente MAUI do ecossistema LifeSync, uma aplicação multiplatform (Android, iOS, macOS, Windows) para gerenciamento de vida pessoal包含:

- **Gerenciamento de Tarefas** (TaskManager)
- **Controle Financeiro** (Financial)
- **Nutrição** (Nutrition)
- **Academia** (Gym) - *Em desenvolvimento*
- **Perfil do Usuário** (Profile)

---

## Estrutura do Projeto

```
ClientApp/
├── LifeSyncApp/
│   ├── Services/
│   │   ├── ApiService/          # Camada HTTP generica
│   │   ├── Auth/                # Autenticação e JWT
│   │   ├── Financial/           # Transações e Categorias
│   │   ├── Nutrition/           # Diários e Refeições
│   │   ├── Profile/             # Perfil do usuário
│   │   ├── TaskManager/          # Tarefas e Etiquetas
│   │   └── UserSession/         # Sessão do usuário
│   ├── ViewModels/
│   │   ├── Auth/                # Login, Registro
│   │   ├── Financial/           # Financeiro
│   │   ├── Nutrition/           # Nutrição
│   │   ├── Profile/             # Perfil
│   │   └── TaskManager/         # Tarefas
│   ├── Views/                   # Páginas XAML
│   ├── Models/                  # Modelos de domínio
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Converters/              # Conversores XAML
│   ├── Helpers/                 # Utilitários
│   ├── Controls/                # Controles customizados
│   ├── App.xaml(.cs)            # Entry point
│   ├── AppShell.xaml(.cs)       # Shell de navegação
│   ├── MauiProgram.cs           # Configuração DI
│   └── MainPage.xaml(.cs)      # Página principal com tabs
├── LifeSyncApp.Tests/           # Testes unitários
└── LifeSyncApp.sln             # Solução
```

---

## Arquitetura

### Padrão Arquitetural

O projeto segue uma arquitetura em camadas com **MVVM** (Model-View-ViewModel):

```
Views (XAML) → ViewModels (C#) → Services (C#) → API (HttpClient)
```

### Injeção de Dependência

O MauiProgram.cs configura todos os serviços via Microsoft.Extensions.DependencyInjection:

```csharp
// ViewModels - Singleton para manter estado entre navegações
builder.Services.AddSingleton<TaskItemsViewModel>();
builder.Services.AddSingleton<FinancialViewModel>();

// Views - Transient para criar nova instância sempre
builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<TaskItemPage>();
```

### Padrões Utilizados

- **Repository Pattern** via ApiService<T> genérico
- **Singleton** para ViewModels que mantêm estado
- **Transient** para Views que são recriadas
- **DelegatingHandler** para interceptar requisições HTTP e adicionar JWT

---

## Tecnologias e Dependências

### Pacotes NuGet

| Pacote | Versão | Propósito |
|--------|--------|-----------|
| CommunityToolkit.Mvvm | 8.4.0 | Toolkit MVVM |
| Microsoft.Extensions.Http | 10.0.1 | HttpClientFactory |
| Microsoft.Maui.Controls | $(MauiVersion) | Controls MAUI |
| System.IdentityModel.Tokens.Jwt | 8.16.0 | Parsing de JWT |
| Microsoft.Extensions.Logging.Debug | 10.0.0 | Logging |

### Frameworks Alvo

```xml
<TargetFrameworks>net10.0-android;net10.0-ios;net10.0-maccatalyst</TargetFrameworks>
```

---

## Problemas Críticos Identificados

### Tabela de Issues

| # | Severidade | Componente | Título | Descrição |
|---|------------|------------|--------|------------|
| **1** | 🔴 CRÍTICO | MauiProgram.cs:63 | URL da API Hardcoded | Base URL `https://api.lifesync.tech` está hardcoded sem opção de configuração por ambiente |
| **2** | 🔴 CRÍTICO | AuthDelegatingHandler.cs:31-40 | Sem tratamento de Refresh Token | Ao receber 401, apenas faz logout sem tentar refresh do token |
| **3** | 🔴 CRÍTICO | UserSession.cs:14-19 | UserId pode ser 0 | Falha no SecureStorage retorna `_userId = 0`, causando chamadas API com userId inválido |
| **4** | 🟠 ALTO | Services/* | Sem Polly/Resiliência | Nenhuma política de retry ou circuit breaker para chamadas HTTP |
| **5** | 🟠 ALTO | ViewModels/* | ViewModels como Singleton | ViewModels Singleton mantêm estado entre navegações, causando possíveis inconsistências |
| **6** | 🟠 ALTO | AcademicPage | Módulo Gym não implementado | Página com placeholder "Coming soon" - backend existe mas frontend não |
| **7** | 🟠 ALTO | LifeSyncApp.Tests | Cobertura de testes mínima | Apenas 1 arquivo de teste, sem cobertura para serviços críticos |
| **8** | 🟡 MÉDIO | BaseViewModel.cs | INotifyPropertyChanged incompleto | `IsBusy` não dispara `OnPropertyChanged` corretamente em setters |
| **9** | 🟡 MÉDIO | ApiService.cs | Sem tratamento de timeout | HttpClient sem timeout configurado ou cancelamento |
| **10** | 🟡 MÉDIO | MauiProgram.cs | HTTPS Certificate bypass | AndroidMessageHandler com `ServerCertificateCustomValidationCallback = true` desabilita validação SSL |

### Detalhamento dos Problemas Críticos

#### Issue #1: URL da API Hardcoded
```csharp
// MauiProgram.cs:63
var baseUrl = "https://api.lifesync.tech";  // VPS produção

// Para desenvolvimento local, descomente abaixo e comente a linha acima:
// var baseUrl = DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.DeviceType == DeviceType.Virtual
//     ? "http://10.0.2.2:6006"  // Emulador Android
//     : "http://192.168.0.36:6006";  // Dispositivo físico
```

**Impacto:** Impossibilita desenvolvimento local eficiente sem alterar código.

#### Issue #2: Sem Refresh Token
```csharp
// AuthDelegatingHandler.cs:30-41
if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
{
    MainThread.BeginInvokeOnMainThread(async () =>
    {
        SecureStorage.RemoveAll();
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("//LoginPage");  // Apenas logout, sem refresh
        }
    });
}
```

**Impacto:** Usuário é deslogado ao expirar o token, mesmo com refresh token válido.

#### Issue #3: UserId Default 0
```csharp
// UserSession.cs:14
_userId = int.TryParse(userIdStr, out var id) ? id : 0;  // 0 é inválido
```

**Impacto:** Chamadas API com `userId=0` retornam dados incorretos ou erros.

---

## Recomendações de Correção

### Prioridade 1 — Críticas (Resolver imediatamente)

| # | Recomendação | Arquivo | Solução Proposta |
|---|--------------|---------|------------------|
| **1** | Externalizar configuração de URL | MauiProgram.cs | Criar `appsettings.json` com `ApiSettings.BaseUrl` e ler via `IConfiguration` |
| **2** | Implementar refresh token automático | AuthDelegatingHandler.cs | Adicionar lógica de `TryRefreshTokenAsync()` antes de fazer logout |
| **3** | Tratar falha de UserId | UserSession.cs | Se `_userId = 0`, forçar re-autenticação ou mostrar erro |

### Prioridade 2 — Altas (Resolver em sprint)

| # | Recomendação | Arquivo | Solução Proposta |
|---|--------------|---------|------------------|
| **4** | Adicionar Polly para resiliência | MauiProgram.cs | Configurar `AddTransientHttpErrorPolicy()` com retry e circuit breaker |
| **5** | Rever uso de Singleton em ViewModels | MauiProgram.cs | Considerar `Transient` ou implementar proper state management |
| **6** | Implementar módulo Gym completo | Views/Academic/ | Substituir placeholder por páginas funcionais integradas ao backend |
| **7** | Expandir cobertura de testes | LifeSyncApp.Tests/ | Adicionar testes para ApiService, AuthService, UserSession, ViewModels principais |

### Prioridade 3 — Médias (Resolver posteriormente)

| # | Recomendação | Arquivo | Solução Proposta |
|---|--------------|---------|------------------|
| **8** | Corrigir BaseViewModel | BaseViewModel.cs | Garantir `OnPropertyChanged` em todas as propriedades |
| **9** | Configurar timeouts HTTP | MauiProgram.cs | Adicionar `Timeout = TimeSpan.FromSeconds(30)` ao HttpClient |
| **10** | Remover bypass de certificado SSL | MauiProgram.cs | Apenas em Debug, com warning; Production deve validar certificados |

### Script de Correção Sugerido

```csharp
// Em MauiProgram.cs - Configuração recomendada
var baseUrl = Configuration["ApiSettings:BaseUrl"] 
    ?? "https://api.lifesync.tech";

// Com suporte a ambiente
#if DEBUG
if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.DeviceType == DeviceType.Virtual)
    baseUrl = "http://10.0.2.2:6006";
#endif
```

---

## Score/Qualidade do Frontend

### Avaliação Geral

| Dimensão | Score (0-10) | Status | Observações |
|----------|--------------|--------|-------------|
| **Arquitetura** | 7.0 | 🟡 Bom | MVVM bem implementado, mas ViewModels Singleton causam concerns |
| **Código Limpo** | 6.5 | 🟡 Bom | Código legível, mas alguns ViewModels estão muito grandes (500+ linhas) |
| **Testabilidade** | 3.0 | 🔴 Fraco | Apenas 1 arquivo de teste, cobertura mínima |
| **Tratamento de Erros** | 5.0 | 🟡 Médio | Logs via Debug.WriteLine, sem error handling consistente |
| **Segurança** | 5.0 | 🟡 Médio | JWT funcionando, mas sem refresh token, SSL bypass em Debug |
| **Performance** | 7.0 | 🟡 Bom | Carregamento paralelo com `Task.WhenAll`, cache local |
| **Manutenibilidade** | 6.5 | 🟡 Bom | DI bem configurado, mas dependências implícitas |
| **UX/UI** | 7.5 | 🟢 Bom | Tab bar customizada, feedback visual, animações |
| **Configurabilidade** | 3.0 | 🔴 Fraco | URL hardcoded, sem appsettings.json |
| **Resiliência** | 4.0 | 🔴 Fraco | Sem retry, sem circuit breaker |

### Score Final

```
Frontend Quality Score: 5.5 / 10.0 (MÉDIO)
```

### Gráfico de Cobertura

| Área | Cobertura Estimada | Prioridade de Testes |
|------|-------------------|---------------------|
| ViewModels | 5% | 🔴 Alta |
| Services | 10% | 🔴 Alta |
| Auth Flow | 15% | 🔴 Alta |
| Navigation | 0% | 🟠 Média |
| Converters | 0% | 🟡 Baixa |

### Principais Pontos de Atenção

1. **Testes** — Prioridade absoluta; cobertura < 10%
2. **Refresh Token** — Impacta diretamente a experiência do usuário
3. **Configuração** — Impossibilita desenvolvimento local
4. **Módulo Gym** — Feature promised mas não implementada

---

## Configuração e Ambiente

### Configuração Atual (Hardcoded)

```csharp
// MauiProgram.cs:63
var baseUrl = "https://api.lifesync.tech";
```

### Configuração Recomendada

Criar `appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://api.lifesync.tech",
    "Timeout": 30
  },
  "Auth": {
    "GoogleClientId": "...",
    "AppleServiceId": "..."
  }
}
```

### Ambientes Suportados

| Ambiente | URL | Status |
|----------|-----|--------|
| Produção (VPS) | https://api.lifesync.tech | ✅ Ativo |
| Desenvolvimento | http://10.0.2.2:6006 (Android Emulator) | ⚠️ Comentar/Descomentar |
| Desenvolvimento | http://192.168.0.36:6006 (Device) | ⚠️ Comentar/Descomentar |

---

## Navegação e Shell

### Estrutura de Navegação

```
AppShell
├── LoginPage (rota: //LoginPage)
├── RegisterPage (rota: //RegisterPage)
└── MainPage (rota: //MainPage)
    └── CustomTabBar
        ├── Tarefas (TaskItemsViewModel)
        ├── Finanças (FinancialViewModel)
        ├── Nutrição (NutritionViewModel)
        ├── Academia (AcademicPage - placeholder)
        └── Perfil (ProfileViewModel)
```

### Rotas de Navegação

| Rota | Tipo | ViewModel Associado |
|------|------|-------------------|
| `MainPage` | Tab | MainPage.xaml.cs |
| `LoginPage` | ShellContent | LoginViewModel |
| `RegisterPage` | ShellContent | RegisterViewModel |
| `TaskItemPage` | Modal | TaskItemsViewModel |
| `ManageTaskItemModal` | Modal | TaskItemsViewModel |
| `FinancialPage` | Tab | FinancialViewModel |
| `CategoriesPage` | Push | CategoriesViewModel |
| `NutritionPage` | Tab | NutritionViewModel |
| `AcademicPage` | Tab | (placeholder) |

---

## Autenticação

### Fluxo de Autenticação

```
1. App.Startup
   ↓
2. App(): OnShellNavigated()
   ↓
3. _authService.IsAuthenticatedAsync()
   ↓ (se não autenticado)
4. Navigate to //LoginPage
```

### JWT Token Flow

```csharp
// AuthDelegatingHandler.cs - Adiciona token em todas as requisições
var token = await SecureStorage.GetAsync("access_token");
request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
```

### Endpoints de Auth

| Endpoint | Método | Uso |
|----------|--------|-----|
| `/auth/login` | POST | Login com email/senha |
| `/auth/register` | POST | Registro de novo usuário |
| `/auth/google-login` | GET | Login social Google (OAuth) |
| `/auth/logout` | POST | Logout e invalidação de token |

### Problema: Sem Refresh Token

O `AuthDelegatingHandler` não implementa refresh token automático:

```csharp
// Problema: Ao receber 401, apenas faz logout
if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
{
    SecureStorage.RemoveAll();
    await Shell.Current.GoToAsync("//LoginPage");
}
```

---

## Serviços

### ApiService<T> (Genérico)

| Método | Uso |
|--------|-----|
| `GetAsync(endpoint)` | GET que retorna `T` |
| `SearchAsync(endpoint)` | GET que retorna `IEnumerable<T>` |
| `PostAsync<T>(endpoint, data)` | POST que retorna `T` |
| `PutAsync(endpoint, data)` | PUT sem retorno |
| `DeleteAsync(endpoint)` | DELETE sem retorno |

### Serviços Específicos

| Serviço | Responsabilidade |
|---------|-----------------|
| `AuthService` | Login, Registro, Logout, Google Auth |
| `UserSession` | Mantém userId da sessão atual |
| `TaskItemService` | CRUD de tarefas |
| `TaskLabelService` | CRUD de etiquetas |
| `TransactionService` | CRUD de transações financeiras |
| `CategoryService` | CRUD de categorias financeiras |
| `NutritionService` | Diários, Refeições, Alimentos, Líquidos |
| `UserProfileService` | Perfil do usuário |

---

## Testes

### Estado Atual

| Categoria | Quantidade | Cobertura |
|-----------|-----------|-----------|
| Test Files | 1 | - |
| ViewModel Tests | 1 (ManageTransactionViewModel) | ~5% |
| Service Tests | 0 | 0% |
| Integration Tests | 0 | 0% |

### Estrutura de Testes Sugerida

```
LifeSyncApp.Tests/
├── ViewModels/
│   ├── Auth/
│   │   ├── LoginViewModelTests.cs
│   │   └── RegisterViewModelTests.cs
│   ├── Financial/
│   │   ├── FinancialViewModelTests.cs
│   │   └── ManageTransactionViewModelTests.cs ✅ (existente)
│   ├── Nutrition/
│   │   └── NutritionViewModelTests.cs
│   └── TaskManager/
│       └── TaskItemsViewModelTests.cs
├── Services/
│   ├── AuthServiceTests.cs
│   ├── UserSessionTests.cs
│   └── ApiServiceTests.cs
└── Helpers/
    └── TestBuilders/
```

### Frameworks de Teste Recomendados

| Framework | Uso |
|-----------|-----|
| xUnit | Test runner |
| Moq | Mocking de dependências |
| FluentAssertions | Assertions expressivas |
| Microsoft.AspNetCore.Mvc.Testing | Testes de integração (opcional) |

---

## Histórico de Alterações

| Data | Versão | Descrição |
|------|--------|-----------|
| 2026-03-23 | 1.0.0 | Criação inicial do documento |
| 2026-02-27 | 0.x | Documento IMPLEMENTATION-TASKS.md (LifeSync geral) |

---

*Documento gerado automaticamente com base na análise do código-fonte do LifeSyncApp*
