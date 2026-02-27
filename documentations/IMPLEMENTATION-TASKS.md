# LifeSync — Tarefas de Implementação Pendentes

> Gerado em: 2026-02-27
> Status do projeto: Backend ~90% completo · MAUI ~70% completo · Blazor WebApp não iniciado

---

## Índice

- [Visão Geral do Status](#visão-geral-do-status)
- [Backend](#backend)
  - [1. Login Social](#1-login-social-google-apple-microsoft)
  - [2. Planos de Assinatura](#2-sistema-de-planos-de-assinatura)
  - [3. Autenticação nas Rotas YARP](#3-habilitar-autenticação-em-todas-as-rotas-do-yarp)
  - [4. Relatórios Financeiros](#4-implementar-relatórios-financeiros)
  - [5. Endpoint de Perfil do Usuário](#5-endpoint-de-perfil-do-usuário)
- [MAUI App](#maui-app)
  - [6. Telas de Autenticação](#6-telas-de-autenticação)
  - [7. Módulo Academia (Gym)](#7-módulo-academia-gym-completo)
  - [8. Perfil e Configurações](#8-perfil-do-usuário-e-configurações)
  - [9. Login Social no App](#9-login-social-no-app-google-e-apple)
  - [10. Dashboard Home](#10-dashboard-home-com-resumo-geral)
  - [11. Tratamento de Erros e Interceptor HTTP](#11-tratamento-de-erros-e-interceptor-http)
  - [12. Gráficos e Relatórios Financeiros](#12-gráficos-e-relatórios-na-tela-financeira)
  - [13. Configuração de Ambientes](#13-configuração-de-ambientes-devprod)
  - [14. Notificações Push Locais](#14-notificações-push-locais)
- [Ordem de Implementação Recomendada](#ordem-de-implementação-recomendada)

---

## Visão Geral do Status

### O que está implementado

| Módulo | Backend | MAUI App |
|--------|---------|----------|
| Autenticação (JWT) | ✅ Completo | ❌ Não iniciado |
| Gerenciamento de Tarefas | ✅ Completo | ✅ Completo |
| Nutrição | ✅ Completo | ✅ Completo |
| Financeiro | ✅ Completo | ✅ Completo |
| Academia/Gym | ✅ Completo | ⚠️ Placeholder "Coming soon" |
| Login Social | ❌ Não iniciado | ❌ Não iniciado |
| Planos de Assinatura | ❌ Não iniciado | ❌ Não iniciado |
| Relatórios Financeiros | ⚠️ Controller vazio | ❌ Não iniciado |
| Perfil do Usuário | ⚠️ Parcial | ❌ Não iniciado |
| Notificações Push | ❌ Não iniciado | ❌ Não iniciado |

### Problemas críticos encontrados

- **userId hardcoded** (`_userId = 1`) em todos os ViewModels do MAUI (TODOs existentes)
- **Autenticação desabilitada** na maioria das rotas YARP (políticas comentadas no `appsettings.json`)
- **URL da API hardcoded** no `MauiProgram.cs` (IPs fixos para emulador e dispositivo)
- **Nenhuma tela de autenticação** no MAUI — o app não tem login/logout

---

## Backend

### 1. Login Social (Google, Apple, Microsoft)

**Objetivo:** Permitir que usuários se autentiquem usando provedores de identidade externos.

**Provedores a implementar:**
- Google (Google Identity Platform)
- Apple (Sign in with Apple)
- Microsoft Account

**Implementação:**

1. Instalar pacotes NuGet:
   - `Microsoft.AspNetCore.Authentication.Google`
   - `Microsoft.AspNetCore.Authentication.MicrosoftAccount`
   - Biblioteca Apple OAuth (ex: `AspNet.Security.OAuth.Apple`)

2. Criar endpoint no `AuthController`:
   ```
   POST /auth/social-login
   Body: { "provider": "google", "idToken": "..." }
   ```
   - Validar o `idToken` com o provedor externo
   - Se usuário com e-mail já existe → gerar JWT
   - Se usuário não existe → criar conta automaticamente → gerar JWT

3. Atualizar entidade `User`:
   - Adicionar campos `ExternalProvider` (string) e `ExternalProviderId` (string)
   - Criar migration EF Core

4. Criar Command `SocialLoginCommand` seguindo padrão CQRS existente

5. Configurar ClientId/ClientSecret via User Secrets (dev) e variáveis de ambiente (prod)

**Arquivos-chave:**
- `Services/Users/Users.API/Controllers/AuthController.cs`
- `Services/Users/Users.Domain/Entities/User.cs`
- `Services/Users/Users.Application/Commands/` → criar `SocialLoginCommand`

---

### 2. Sistema de Planos de Assinatura

**Objetivo:** Monetizar o app com planos Free, Premium e Pro integrados ao Stripe.

**Planos sugeridos:**

| Plano | Preço | Recursos |
|-------|-------|----------|
| Free | Grátis | Tarefas limitadas (10), sem relatórios avançados |
| Premium | R$ 19,90/mês | Todos os módulos sem limites |
| Pro | R$ 39,90/mês | Relatórios avançados, exportação, suporte prioritário |

**Implementação:**

1. Criar entidades:
   ```
   SubscriptionPlan (Id, Name, Price, BillingCycle, Features[])
   UserSubscription (UserId, PlanId, Status, StartDate, EndDate, StripeSubscriptionId)
   ```

2. Criar `SubscriptionsController` com endpoints:
   ```
   GET  /subscriptions/plans            → listar planos disponíveis
   GET  /subscriptions/current          → plano atual do usuário logado
   POST /subscriptions                  → iniciar assinatura
   POST /subscriptions/cancel           → cancelar assinatura
   ```

3. Integração com Stripe:
   - Instalar `Stripe.net`
   - Criar `StripeService` para: criar Customer, criar Subscription, cancelar
   - Webhook `POST /payments/webhook` para processar eventos:
     - `invoice.payment_succeeded`
     - `customer.subscription.deleted`
     - `customer.subscription.updated`
   - Salvar `StripeCustomerId` na entidade `User`

4. Controle de acesso por plano:
   - Adicionar claim `subscription_plan` ao JWT gerado no login
   - Criar policy `RequiresPremium` para rotas exclusivas
   - Aplicar via YARP ou nos controllers

5. Seed inicial dos planos no banco via EF Core

**Arquivos-chave:**
- `Services/Users/` (extensão) ou criar `Services/Subscription/` (novo microserviço)
- `Services/ApiGateways/YarpApiGateway/appsettings.json`

---

### 3. Habilitar Autenticação em Todas as Rotas do YARP

**Objetivo:** Proteger os endpoints da API exigindo JWT válido em todas as chamadas.

**Implementação:**

1. **`appsettings.json` do YARP** — habilitar `RequireAuthentication` em:
   - `/taskmanager-service/*`
   - `/nutrition-service/*`
   - `/financial-service/*`
   - `/users-service/*`
   - `/gym-service/*`

   Manter públicas: `POST /auth/login`, `POST /auth/register`, `POST /auth/forgot-password`, `GET /auth/confirm-email`, `POST /auth/social-login`

2. **Extração do UserId nos serviços:**
   - Criar extension method `GetCurrentUserId()` no `BuildingBlocks` via `IHttpContextAccessor`
   - Substituir todos os `userId` hardcoded pelo userId real do JWT em todos os serviços
   - Injetar `IHttpContextAccessor` nos handlers que precisam do userId

**Arquivos com hardcode (TODOs existentes):**
- `ClientApp/LifeSyncApp/.../CategoriesViewModel.cs` — linha 11
- `ClientApp/LifeSyncApp/.../NutritionViewModel.cs` — linha 14
- Verificar todos os controllers/handlers do backend

---

### 4. Implementar Relatórios Financeiros

**Objetivo:** Preencher o `ReportsController` que existe mas está vazio no serviço Financial.

**Endpoints a implementar:**

```
GET /financial-service/reports/summary?month=&year=
→ Resumo mensal: total receitas, despesas, saldo

GET /financial-service/reports/by-category?startDate=&endDate=
→ Gastos agrupados por categoria com percentual

GET /financial-service/reports/cashflow?months=6
→ Fluxo de caixa dos últimos N meses (para gráfico de barras)

GET /financial-service/reports/top-expenses?limit=5
→ Top categorias de maior gasto
```

**Implementação seguindo padrão CQRS:**
- Criar Queries em `Financial.Application/Reports/Queries/`
- Criar DTOs específicos para cada relatório
- Registrar no `ReportsController` seguindo padrão dos outros controllers

**Arquivos-chave:**
- `Services/Financial/Financial.API/Controllers/ReportsController.cs`
- `Services/Financial/Financial.Application/` → criar pasta `Reports/`

---

### 5. Endpoint de Perfil do Usuário

**Objetivo:** Garantir endpoints completos de perfil para suportar a tela de perfil do MAUI.

**Endpoints necessários:**

```
GET  /users-service/users/profile        → dados do usuário logado
PUT  /users-service/users/profile        → atualizar nome e foto
PUT  /auth/change-password               → alterar senha (verificar se existe)
POST /users-service/users/profile/picture → upload de foto (multipart)
DELETE /users-service/users/account      → deletar conta (LGPD)
```

**Atualizar `UserDTO` de resposta:**
- `SubscriptionPlan` (Free/Premium/Pro)
- `SubscriptionExpiresAt`
- `ProfilePictureUrl`
- `CreatedAt`

**Arquivos-chave:**
- `Services/Users/Users.API/Controllers/UsersController.cs`

---

## MAUI App

### 6. Telas de Autenticação

**Objetivo:** Implementar fluxo completo de autenticação no app — atualmente inexistente.

**Telas a criar:**

**`Pages/Auth/LoginPage.xaml`**
- Campos: Email, Senha (com show/hide)
- Botão "Entrar"
- Links: "Esqueci minha senha" e "Criar conta"
- Botões de login social: Google e Apple
- Indicador de carregamento durante login

**`Pages/Auth/RegisterPage.xaml`**
- Campos: Nome, Email, Senha, Confirmar Senha
- Validação de senha forte
- Feedback de sucesso (orientar verificar e-mail)

**`Pages/Auth/ForgotPasswordPage.xaml`**
- Campo: Email
- Botão "Enviar link de recuperação"
- Tela de confirmação de envio

**`Services/AuthService.cs`**
```csharp
LoginAsync(email, password)          // POST /auth/login → salva JWT no SecureStorage
RegisterAsync(name, email, password) // POST /auth/register
SocialLoginAsync(provider, idToken)  // POST /auth/social-login
LogoutAsync()                        // remove token do SecureStorage
GetCurrentUserIdAsync()              // decodifica JWT → retorna userId
IsAuthenticatedAsync()               // verifica se token existe e não expirou
```

**`Services/CurrentUserService.cs`** — singleton que expõe `UserId`, `UserName`, `SubscriptionPlan`

**`App.xaml.cs`** — verificar autenticação no startup:
- Se não autenticado → navegar para `LoginPage`
- Após login → navegar para shell principal com tabs

**Atualizar todos os ViewModels:** substituir `_userId = 1` por `await _currentUserService.GetUserIdAsync()`

**Arquivos a criar:**
```
Pages/Auth/LoginPage.xaml(.cs)
Pages/Auth/RegisterPage.xaml(.cs)
Pages/Auth/ForgotPasswordPage.xaml(.cs)
ViewModels/Auth/LoginViewModel.cs
ViewModels/Auth/RegisterViewModel.cs
ViewModels/Auth/ForgotPasswordViewModel.cs
Services/AuthService.cs
Services/CurrentUserService.cs
```

---

### 7. Módulo Academia (Gym) Completo

**Objetivo:** Substituir o placeholder "Coming soon" por um módulo funcional completo. O backend já está 100% implementado.

**Telas a criar:**

**`AcademicPage`** — Dashboard principal
- Resumo: treinos esta semana, última sessão, rotina ativa
- Botão "Iniciar Treino" rápido
- Lista de últimas sessões com data e duração
- Acesso para: Exercícios, Rotinas, Histórico

**`ExercisesPage` + `ManageExerciseModal`**
- Lista com filtros por grupo muscular e tipo (chips)
- Card: nome, grupo muscular, tipo, equipamento
- Modal criar/editar: nome, descrição, MuscleGroup, ExerciseType, EquipmentType (pickers)

**`RoutinesPage` + `ManageRoutineModal` + `RoutineDetailPage`**
- Lista de rotinas com botão "Definir como Ativa"
- `RoutineDetailPage`: exercícios da rotina com séries/reps/carga
- `ManageRoutineExerciseModal`: definir séries, reps e carga para cada exercício

**`TrainingSessionPage` + `ActiveSessionPage`**
- Histórico de sessões passadas
- `ActiveSessionPage`: treino ativo em tempo real
  - Cronômetro de treino
  - Exercícios da rotina listados
  - Registro de séries completadas (reps + carga)
  - Botão "Finalizar Treino" → salva sessão + exercícios completos

**`Services/GymService.cs`**
- Todos os métodos CRUD para Exercises, Routines, RoutineExercises, TrainingSessions, CompletedExercises

**DTOs a criar:**
```
ExerciseDTO, CreateExerciseDTO, UpdateExerciseDTO
RoutineDTO, CreateRoutineDTO, RoutineDetailDTO
RoutineExerciseDTO, CreateRoutineExerciseDTO
TrainingSessionDTO, CreateTrainingSessionDTO
CompletedExerciseDTO, CreateCompletedExerciseDTO
```

**ViewModels a criar:**
```
ViewModels/Academic/AcademicViewModel.cs
ViewModels/Academic/ExercisesViewModel.cs
ViewModels/Academic/RoutinesViewModel.cs
ViewModels/Academic/RoutineDetailViewModel.cs
ViewModels/Academic/TrainingSessionViewModel.cs
ViewModels/Academic/ActiveSessionViewModel.cs
```

Registrar `GymService` no `MauiProgram.cs`

---

### 8. Perfil do Usuário e Configurações

**Objetivo:** Adicionar aba de perfil no shell com gerenciamento de conta e assinatura.

**Telas a criar:**

**`ProfilePage`** — nova aba no Shell
- Avatar (inicial do nome ou foto)
- Nome, e-mail e badge do plano (Free / Premium / Pro)
- Seções: Conta, Assinatura, Preferências, "Sair"

**`EditProfilePage`**
- Campos: Nome, foto de perfil
- `PUT /users-service/users/profile`

**`ChangePasswordPage`**
- Campos: Senha atual, Nova senha, Confirmar
- `PUT /auth/change-password`

**`SubscriptionPage`**
- Cards dos planos (Free, Premium, Pro) com destaque no atual
- Botão "Assinar" / "Fazer Upgrade"
- WebView ou deep link para checkout do Stripe

**`ProfileViewModel.cs`**
- Carregar dados do usuário via API
- `LogoutAsync()` → `AuthService.LogoutAsync()` → navegar para `LoginPage`

**Atualizar `AppShell.xaml`:**
- Adicionar aba "Perfil" com ícone de pessoa
- Ordem sugerida: Home · Tarefas · Finanças · Nutrição · Academia · Perfil

---

### 9. Login Social no App (Google e Apple)

**Objetivo:** Botões de login social na `LoginPage` integrados ao backend.

**Implementação:**

1. Instalar pacotes NuGet:
   - `Plugin.GoogleClient` ou `GoogleSignIn.Maui`
   - Para Apple: `AuthenticationServices` nativo via MAUI
   - Alternativa: `Microsoft.Identity.Client` (MSAL) para múltiplos provedores

2. Configurações por plataforma:
   - Android: `google-services.json`, redirect URI
   - iOS: `Info.plist` com URL Schemes para Google e Apple

3. `Services/GoogleAuthService.cs`
   - `SignInWithGoogleAsync()` → retorna `idToken` → passa para `AuthService.SocialLoginAsync("google", idToken)`

4. `Services/AppleAuthService.cs`
   - `SignInWithAppleAsync()` → retorna credenciais → passa para `AuthService.SocialLoginAsync("apple", idToken)`

5. Atualizar `LoginViewModel`:
   - `LoginWithGoogleCommand`
   - `LoginWithAppleCommand` (visível apenas em iOS/macOS via `OnPlatform`)

6. Atualizar `LoginPage.xaml`:
   - Botão Google (cor `#4285F4`, ícone Google)
   - Botão Apple (apenas iOS via `OnPlatform`)
   - Separador "— ou —" entre login tradicional e social

**Depende de:** Tarefa #1 (Login Social no Backend)

---

### 10. Dashboard Home com Resumo Geral

**Objetivo:** Tela inicial que consolida informações de todos os módulos.

**`Pages/Home/HomePage.xaml` + `HomeViewModel.cs`**

Widgets/cards no dashboard:
- **Saudação:** "Bom dia, Victor!" + data atual
- **Tarefas:** Pendentes hoje + vencidas (badge vermelho)
- **Nutrição:** Progresso de calorias (barra circular), meta vs consumido
- **Finanças:** Saldo do mês + última transação
- **Academia:** Último treino / streak de dias

Ações rápidas:
- Botões flutuantes para adicionar transação, tarefa e refeição

**Implementação:**
- Carregar dados em paralelo via `Task.WhenAll`
- Pull-to-refresh
- Navegação para módulo específico ao clicar em cada widget
- Skeleton loading enquanto dados carregam

**Atualizar `AppShell.xaml`:**
- Home como primeira aba com ícone de casa

---

### 11. Tratamento de Erros e Interceptor HTTP

**Objetivo:** Substituir alertas genéricos por tratamento estruturado com renovação automática de token.

**`Handlers/AuthenticatedHttpHandler.cs`** — `DelegatingHandler`
- Adiciona `Authorization: Bearer {token}` automaticamente em todas as requisições
- Intercepta 401 → tenta refresh token → se falhar → logout + redireciona para `LoginPage`
- Intercepta 403 → mensagem "Sem permissão" ou redireciona para `SubscriptionPage`
- Intercepta erros de rede → mensagem "Sem conexão"

**`Models/ApiException.cs`**
- Deserializa erros da API (`errors`, `message`, `statusCode`)
- Extrai mensagem amigável de validação

**Atualizar `ApiService<T>`:**
- Usar `AuthenticatedHttpHandler` via `HttpClientFactory`
- Tratar e relançar `ApiException`

**Atualizar `MauiProgram.cs`:**
- Registrar `AuthenticatedHttpHandler` como Transient
- `AddHttpClient` usando o handler em cadeia

**`BaseViewModel` melhorado:**
- `IsBusy`, `ErrorMessage` observáveis
- `ExecuteAsync(func)` com try/catch e controle de `IsBusy`
- Verificar `Connectivity.Current.NetworkAccess` antes de chamadas HTTP

---

### 12. Gráficos e Relatórios na Tela Financeira

**Objetivo:** Visualizações de dados financeiros com gráficos interativos.

**Instalar:** `LiveChartsCore.SkiaSharpView.Maui` (recomendado)

**`Pages/Financial/FinancialReportsPage.xaml`**
- Seletor de período: Mês / Trimestre / Ano
- **Gráfico de pizza:** Gastos por categoria (%)
- **Gráfico de barras:** Receitas vs Despesas por mês (últimos 6 meses)
- **Lista:** Top 5 categorias com valor e barra de progresso
- **Cards:** Saldo, total receitas, total despesas

**`ViewModels/Financial/FinancialReportsViewModel.cs`**
- Consumir endpoints de relatórios do backend
- Transformar dados para formato de gráfico
- Filtros de período com `DatePicker`

**Atualizar `FinancialPage`:**
- Botão/aba "Relatórios" → navega para `FinancialReportsPage`

> ⚠️ **Depende de:** Tarefa #4 (Relatórios Financeiros no Backend)

---

### 13. Configuração de Ambientes (Dev/Prod)

**Objetivo:** Remover URL e configurações hardcoded, suportando múltiplos ambientes.

**Implementação:**

1. Criar `appsettings.json` como EmbeddedResource:
   ```json
   {
     "ApiSettings": {
       "BaseUrl": "https://api.lifesync.com",
       "Timeout": 30
     },
     "Auth": {
       "GoogleClientId": "...",
       "AppleServiceId": "..."
     }
   }
   ```

2. Criar `appsettings.Development.json` para dev local

3. Criar `AppSettings.cs` — classe fortemente tipada registrada como singleton no DI

4. Atualizar `MauiProgram.cs`:
   - Ler `BaseUrl` do `AppSettings` ao invés de hardcode
   - Usar `#if ANDROID` apenas para resolver endereço de loopback do emulador

5. Suporte a variável de ambiente `LIFESYNC_API_URL` para override

6. Adicionar tela "Sobre" em Configurações com versão do app e ambiente atual

---

### 14. Notificações Push Locais

**Objetivo:** Lembretes locais para tarefas vencendo, horário de treino e refeições.

**Instalar:** `Plugin.LocalNotification`

**Configurações por plataforma:**
- Android: canal de notificação no `MainApplication.cs`; permissões `RECEIVE_BOOT_COMPLETED`, `SCHEDULE_EXACT_ALARM`
- iOS: solicitar permissão no startup; `NSUserNotificationUsageDescription`

**`Services/NotificationService.cs`**
```csharp
ScheduleTaskReminderAsync(taskItem)     // notificação 1h antes do DueDate
ScheduleWorkoutReminderAsync(time)      // lembrete diário de treino
ScheduleNutritionReminderAsync()        // lembrete de registrar refeições
CancelNotificationAsync(id)
CancelAllAsync()
```

**`Pages/Settings/NotificationsPage.xaml`**
- Toggle: "Lembrete de tarefas vencendo"
- Toggle: "Lembrete de treino" + `TimePicker`
- Toggle: "Lembrete de refeições"
- Salvar preferências via `Preferences` do MAUI

**Integração com `ManageTaskItemModal`:**
- Ao criar tarefa com DueDate → oferecer agendar lembrete

---

## Ordem de Implementação Recomendada

### Fase 1 — Fundação (crítico)

```
[Backend] #3 Habilitar autenticação nas rotas YARP
[MAUI]    #6 Telas de autenticação (Login/Registro)
[Backend] #5 Endpoint de perfil do usuário
```

### Fase 2 — Login Social e Assinaturas

```
[Backend] #1 Login Social (Google, Apple, Microsoft)
[MAUI]    #9 Login Social no app
[Backend] #2 Planos de assinatura (Stripe)
[MAUI]    #8 Perfil e gerenciamento de assinatura
```

### Fase 3 — Módulos Faltantes

```
[MAUI]    #7 Módulo Academia completo
[MAUI]    #10 Dashboard Home
[MAUI]    #11 Tratamento de erros e interceptor HTTP
[MAUI]    #13 Configuração de ambientes
```

### Fase 4 — Polimento e Features Avançadas

```
[Backend] #4 Relatórios financeiros
[MAUI]    #12 Gráficos e relatórios financeiros
[MAUI]    #14 Notificações push locais
```

---

*LifeSync — Organize sua vida em um único lugar.*
