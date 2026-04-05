# 👤 Users Service

Microserviço responsável pelo **gerenciamento de usuários, autenticação e autorização** no LifeSync.

> **Stack:** ASP.NET Core Identity · JWT + Refresh Token · Google OAuth · PostgreSQL · RabbitMQ  
> **Porta:** `5001` · **Schema:** `users`  
> **Padrões:** CQRS · DDD · Result Pattern · Clean Architecture

## Índice

- [Visão Geral](#visão-geral)
- [Estrutura de Pastas](#estrutura-de-pastas)
- [Domínio](#domínio)
- [Aplicação](#aplicação)
- [Infraestrutura](#infraestrutura)
- [API](#api)
- [Configuração](#configuração)
- [Dependências](#dependências)
- [📚 Documentação Relacionada](#-documentação-relacionada)

---

## Visão Geral

O Users Service é o serviço central de identidade do LifeSync. Gerencia o ciclo de vida completo dos usuários — registro, login, recuperação de senha, gerenciamento de perfil e controle de papéis (RBAC). Utiliza **ASP.NET Core Identity** como base e emite **tokens JWT** consumidos por todos os demais microserviços.

### Responsabilidades

- Registro e login de usuários com emissão de JWT + Refresh Token
- Recuperação e redefinição de senha via e-mail
- Confirmação de e-mail
- Gerenciamento de perfil (nome, contato, data de nascimento)
- Sistema de papéis (RBAC) via ASP.NET Identity
- Publicação do evento `UserRegisteredEvent` no RabbitMQ ao registrar novo usuário

---

## Estrutura de Pastas

```
Users/
├── Users.API/
│   ├── Controllers/
│   │   ├── AuthController.cs        # Login, registro, senha
│   │   └── UsersController.cs       # CRUD de usuários
│   ├── Program.cs
│   ├── appsettings.json
│   └── Users.API.csproj
├── Users.Application/
│   ├── Contracts/
│   │   ├── IAuthService.cs
│   │   ├── IEmailService.cs
│   │   ├── IRoleService.cs
│   │   ├── ITokenGenerator.cs
│   │   └── IUserService.cs
│   ├── DTOs/
│   │   ├── Auth/                    # LoginDTO, SignUpRequest, AuthResult...
│   │   ├── Email/                   # EmailMessageDTO
│   │   ├── Role/                    # RoleDTO
│   │   └── User/                    # UserDTO, UpdateUserDTO, UserSummaryDTO
│   ├── Features/
│   │   ├── Auth/Commands/           # Login, SignUp, Logout, ChangePassword...
│   │   └── Users/
│   │       ├── Commands/            # Update, Delete
│   │       └── Queries/             # GetById, GetAll
│   ├── Mapping/UserMapper.cs
│   └── Users.Application.csproj
├── Users.Domain/
│   ├── Entities/User.cs
│   ├── Events/UserRegisteredEvent.cs
│   ├── Errors/                      # UserErrors, NameErrors, ContactErrors
│   ├── ValueObjects/
│   │   ├── Name.cs
│   │   └── Contact.cs
│   └── Users.Domain.csproj
└── Users.Infrastructure/
    ├── Data/
    │   ├── ApplicationDbContext.cs
    │   └── MigrationHostedService.cs
    ├── Migrations/
    ├── Services/
    │   ├── AuthService.cs
    │   ├── EmailService.cs
    │   ├── RoleService.cs
    │   ├── TokenGenerator.cs
    │   └── UserService.cs
    ├── Settings/
    │   ├── JwtSettings.cs
    │   └── RefreshToken.cs
    ├── Smtp/SmtpSettings.cs
    └── Users.Infrastructure.csproj
```

---

## Domínio

### Entidade: `User`

Estende `IdentityUser<int>` e `IBaseEntity<int>`.

| Propriedade | Tipo | Descrição |
|---|---|---|
| `Name` | `Name` (ValueObject) | Primeiro e último nome |
| `Contact` | `Contact` (ValueObject) | E-mail com validação via regex |
| `BirthDate` | `DateOnly?` | Data de nascimento |
| `CreatedAt` | `DateTime` | Data de criação (UTC) |
| `UpdatedAt` | `DateTime?` | Última atualização |
| `IsDeleted` | `bool` | Soft delete |
| `LastLoginAt` | `DateTime?` | Último login |
| `IsActive` | `bool` | Usuário ativo (default: true) |
| `RefreshToken` | `string?` | Token de renovação |
| `RefreshTokenExpiryTime` | `DateTime` | Validade do refresh token |
| `DomainEvents` | `IReadOnlyCollection<IDomainEvent>` | Eventos acumulados |

**Métodos de domínio:**

| Método | Descrição |
|---|---|
| `UpdateProfile(name, contact)` | Atualiza nome e contato |
| `Deactivate()` | Desativa a conta |
| `Activate()` | Ativa a conta |
| `UpdateLastLogin()` | Registra data do último login |
| `MarkAsUpdated()` | Atualiza `UpdatedAt` |
| `MarkAsDeleted()` | Soft delete (`IsDeleted = true`) |
| `AddDomainEvent(event)` | Adiciona evento ao aggregate |
| `ClearDomainEvents()` | Limpa os eventos |

---

### Value Object: `Name`

```
Name
├── FirstName: string   (obrigatório, não pode ser vazio)
├── LastName: string    (obrigatório, não pode ser vazio)
└── FullName: string    (computed: "FirstName LastName")
```

---

### Value Object: `Contact`

```
Contact
└── Email: string  (validado via regex: ^[^@\s]+@[^@\s]+\.[^@\s]+$)
```

---

### Evento de Domínio: `UserRegisteredEvent`

Publicado no RabbitMQ (exchange: `user_exchange`, routing key: `user.registered`) quando um novo usuário se registra.

| Propriedade | Tipo |
|---|---|
| `UserId` | `int` |
| `Email` | `string` |

---

### Erros de Domínio

| Classe | Erros definidos |
|---|---|
| `UserErrors` | Validação, conflito, autenticação, operações CRUD |
| `NameErrors` | Nome obrigatório, formato inválido |
| `ContactErrors` | E-mail com formato inválido |

---

## Aplicação

### Commands

| Command | Handler | Retorno | Descrição |
|---|---|---|---|
| `SignUpCommand` | `SignUpCommandHandler` | `AuthResult` | Cria usuário, gera tokens, publica `UserRegisteredEvent` |
| `LoginCommand` | `LogInCommandHandler` | `AuthResult` | Autentica e gera tokens |
| `LogoutCommand` | `LogoutCommandHandler` | `LogoutResult` | Invalida refresh token |
| `ChangePasswordCommand` | `ChangePasswordCommandHandler` | `Result` | Altera senha autenticada |
| `ForgotPasswordCommand` | `ForgotPasswordCommandHandler` | `Result` | Envia e-mail de recuperação |
| `ResetPasswordCommand` | `ResetPasswordCommandHandler` | `ResetPasswordResult` | Redefine senha com token |
| `SendEmailConfirmationCommand` | `SendEmailConfirmationCommandHandler` | `Result` | Envia e-mail de confirmação |
| `UpdateUserCommand` | `UpdateUserCommandHandler` | `UpdateUserCommandResult` | Atualiza perfil do usuário |
| `DeleteUserCommand` | `DeleteUserCommandHandler` | `DeleteUserResult` | Remove usuário |

---

### Queries

| Query | Handler | Retorno | Descrição |
|---|---|---|---|
| `GetUserQuery` | `GetUserQueryHandler` | `GetUserQueryResult` | Retorna dados de um usuário |
| `GetAllUsersQuery` | `GetAllUsersQueryHandler` | `GetAllUsersQueryResult` | Retorna todos os usuários com papéis |

---

### DTOs

#### Auth

| DTO | Campos |
|---|---|
| `LoginDTO` | `UserNameOrEmail`, `Password`, `RememberMe` |
| `SignUpRequest` | `FirstName`, `LastName`, `Email`, `Password` |
| `AuthResult` | `AccessToken`, `RefreshToken`, `UserDTO` |
| `ChangePasswordDTO` | `CurrentPassword`, `NewPassword`, `ConfirmPassword` |
| `ForgotPasswordDTO` | `Email` |
| `ResetPasswordDTO` | `Email`, `Token`, `NewPassword`, `ConfirmPassword` |
| `ConfirmEmailDTO` | `UserId`, `Token` |

#### Usuário

| DTO | Campos |
|---|---|
| `UserDTO` | `Id`, `FirstName`, `LastName`, `FullName`, `Email`, `BirthDate`, `CreatedAt`, `LastLoginAt`, `IsActive`, `Roles` |
| `UpdateUserDTO` | `Id`, `FirstName`, `LastName`, `Email`, `BirthDate` |
| `UserSummaryDTO` | `Id`, `FullName`, `UserName`, `Email` |

#### E-mail

| DTO | Campos |
|---|---|
| `EmailMessageDTO` | `To`, `Subject`, `Body`, `IsHtml`, `Attachments`, `Token`, `CallbackUrl` |

---

### Contratos de Serviço

#### `IAuthService`

```
LoginAsync(email, password)
SignUpAsync(firstName, lastName, email, password)
LogoutAsync(user)
UpdateRefreshTokenAsync(userId, refreshToken)
RevokeRefreshTokenAsync(refreshToken)
SendEmailConfirmationAsync(email)
ConfirmEmailAsync(userId, token)
SendPasswordResetAsync(email)
ResetPasswordAsync(userId, token, newPassword)
ChangePasswordAsync(user, currentPassword, newPassword)
```

#### `IUserService`

```
GetCurrentUserDetailsAsync(user)
UpdateCurrentUserProfileAsync(user, firstName, lastName, email)
ChangeCurrentUserPasswordAsync(user, currentPassword, newPassword)
DeleteCurrentUserAsync(user)
GetUserDetailsAsync(userId)
GetAllUsersAsync()
GetAllUsersDetailsAsync()
IsUserEmailUniqueAsync(email)
UpdateUserProfileAsync(dto)
DeleteUserAsync(userId)
```

#### `ITokenGenerator`

```
GenerateToken(userId, email, roles, cancellationToken, extraClaims?)
GenerateRefreshToken()
GetPrincipalFromExpiredToken(token)
```

#### `IRoleService`

```
GetUserRolesAsync(userId)
GetCurrentUserRolesAsync(user)
CreateRoleAsync(roleName)
EditRoleAsync(roleName, newRoleName)
DeleteRoleAsync(roleName)
AssignUserToRolesAsync(userId, roles)
RemoveUserFromRolesAsync(userId, roles)
UpdateUserRolesAsync(userId, roles)
```

---

## Infraestrutura

### `ApplicationDbContext`

Herda de `IdentityDbContext<User, IdentityRole<int>, int>`.

- `Name` (ValueObject) configurado como owned entity (`FirstName`, `LastName`)
- `Contact` (ValueObject) configurado como owned entity (`Email`)
- `FullName` marcado como não mapeado (`Ignore`)

### Migrations

| Migration | Data |
|---|---|
| `20250502235433_initialMigration` | 2025-05-02 |
| `20250503001611_initialMigration1` | 2025-05-03 |
| `20250503001714_initialMigration2` | 2025-05-03 |
| `20251018025839_changeModel` | 2025-10-18 |

### `MigrationHostedService`

Executa as migrations automaticamente na inicialização via `IHostedService`.

### `TokenGenerator`

- Gera JWT com claims: `NameIdentifier`, `Name`, `GivenName`, `Surname`, `UserId`
- Gera Refresh Token como Base64 de bytes aleatórios
- Valida tokens expirados para rotação de refresh tokens

### `EmailService`

- Envia e-mails via SMTP com suporte a HTML e anexos
- Templates embutidos para recuperação de senha e confirmação de e-mail

### Configuração de Identity

| Parâmetro | Valor |
|---|---|
| Mínimo de caracteres na senha | 6 |
| Requer letras minúsculas | Sim |
| Caracteres únicos mínimos | 1 |
| Tentativas antes de lockout | 5 |
| Tempo de lockout | 5 minutos |
| E-mail único | Obrigatório |

---

## API

### `AuthController` — `/api/auth`

| Método | Rota | Body / Params | Auth | Descrição |
|---|---|---|---|---|
| POST | `/login` | `LoginCommand` | Anônimo | Autentica e retorna tokens |
| POST | `/register` | `SignUpCommand` | Anônimo | Registra novo usuário |
| POST | `/logout` | — | Autenticado | Invalida sessão |
| POST | `/send-email-confirmation` | `SendEmailConfirmationCommand` | Autenticado | Envia confirmação de e-mail |
| POST | `/forgot-password` | `ForgotPasswordCommand` | Anônimo | Inicia recuperação de senha |
| POST | `/reset-password` | `ResetPasswordCommand` | Anônimo | Redefine senha com token |
| POST | `/change-password` | `ChangePasswordCommand` | Autenticado | Altera senha |

---

### `UsersController` — `/api/users`

| Método | Rota | Params | Auth | Descrição |
|---|---|---|---|---|
| GET | `/{userId:int}` | `userId` | Autenticado | Retorna dados do usuário |
| GET | `/` | — | Autenticado | Lista todos os usuários |
| PUT | `/{userId:int}` | `userId` + body | Autenticado | Atualiza perfil |

---

### Health Check

```
GET /health
→ { "status": "healthy", "service": "Users", "timestamp": "...", "environment": "..." }
```

---

### Padrão de Resposta

Todos os endpoints retornam `HttpResult<object>`:

```json
{
  "success": true,
  "statusCode": 200,
  "data": { ... },
  "errors": []
}
```

---

## Problemas Críticos (Code Review)

> Fonte: [USERS_CODE_REVIEW.md](../code-reviews/USERS_CODE_REVIEW.md)
> Data do Review: 03/03/2026

| Severidade | ID | Problema | Arquivo | Impacto |
|---|---|---|---|---|
| CRÍTICO | 1.1 | Política de senha fraca (OWASP Non-Compliant) | `DependencyInjection.cs` | Senha "aaaaaa" aceita. Requer 12+ chars com mixed case e special chars |
| CRÍTICO | 1.2 | JWT Secret Hardcoded | `appsettings.json` | Chave exposta no controle de versão |
| CRÍTICO | 1.3 | Sem Token Blacklist/Revogação | `AuthService.cs` | Token roubado permanece válido até expirar (60 min) |
| CRÍTICO | 2.1 | Soft Delete Não Filtrado | `UserService.cs` | Usuários deletados aparecem nas listas e podem fazer login |
| CRÍTICO | 4.1 | N+1 em `GetAllUsersDetailsAsync` | `UserService.cs` | 1000 usuários = 1001 queries |
| CRÍTICO | 5.1 | Bug no `UsersController.UpdateUser` | `UsersController.cs` | Variável `updateCommand` ignorada — usa command original |
| ALTO | 1.4 | `LastLoginAt` nunca atualizado | `AuthService.LoginAsync()` | Campo permanece null eternamente |
| ALTO | 1.5 | Email alterado sem verificação | `UpdateUserProfileAsync()` | Risco de account takeover |
| ALTO | 1.6 | Refresh Token Expiry hardcoded | `AuthService.cs` | Ignora `JwtSettings.RefreshTokenExpiryDays` |
| ALTO | 2.2 | BirthDate aceita datas futuras | Domain `User` | Validação existe mas não é aplicada |
| ALTO | 2.3 | `DeleteUserCommandHandler` vazio | `DeleteUserCommandHandler.cs` | Funcionalidade não implementada |
| ALTO | 5.2 | Inconsistência de tipos de ID | DTOs | `UserDTO.Id` (string) vs `UpdateUserDTO.Id` (int) |
| ALTO | 6.1 | Sem HTTPS Enforcement | `Program.cs` | Ausência de `UseHsts()` e `UseHttpsRedirection()` |
| ALTO | 6.2 | CORS Permissivo | `Program.cs` | `AllowAnyMethod()` e `AllowAnyHeader()` |
| ALTO | 6.3 | Sem Rate Limiting | Controllers | Vulnerável a brute force |

---

## Recomendações de Correção

### Prioridade 1 — Críticos (Esforço Total: ~8h)

| # | Recomendação | Arquivo | Tempo |
|---|---|---|---|
| 1 | Fortalecer política de senha para OWASP (12+ chars, mixed case, special chars) | `DependencyInjection.cs` | 15 min |
| 2 | Mover JWT Key para variável de ambiente | `appsettings.json` | 30 min |
| 3 | Filtrar `IsDeleted` em todas as queries do `UserService` | `UserService.cs` | 1h |
| 4 | Corrigir bug no `UsersController.UpdateUser` (usar `updateCommand`) | `UsersController.cs` | 10 min |
| 5 | Corrigir N+1 com JOIN em `GetAllUsersDetailsAsync` | `UserService.cs` | 2h |
| 6 | Implementar token blacklist (Redis ou DB) | `AuthService.cs` | 4h |

### Prioridade 2 — Altos (Esforço Total: ~10h)

| # | Recomendação | Arquivo | Tempo |
|---|---|---|---|
| 7 | Chamar `UpdateLastLogin()` após login bem-sucedido | `AuthService.LoginAsync()` | 15 min |
| 8 | Exigir verificação de email antes de confirmar mudança | `UpdateUserProfileAsync()` | 4h |
| 9 | Implementar `DeleteUserCommandHandler` | `DeleteUserCommandHandler.cs` | 2h |
| 10 | Validar `BirthDate` no construtor do `User` | `User.cs` | 15 min |

---

## 📚 Documentação Relacionada

| Tipo | Documento | Descrição |
|------|-----------|----------|
| 📋 Code Review | [USERS_CODE_REVIEW.md](../code-reviews/USERS_CODE_REVIEW.md) | Revisão detalhada de código com issues por severidade |
| 📧 Integração | [notification-service.md](notification-service.md) | Consome o evento `UserRegisteredEvent` publicado por este serviço |
| 🔐 Google Auth | [GOOGLE-AUTH-IMPLEMENTATION.md](../deployment/GOOGLE-AUTH-IMPLEMENTATION.md) | Guia de implementação do login OAuth com Google |
| 🔧 Building Blocks | [building-blocks.md](building-blocks.md) | Bibliotecas compartilhadas (CQRS, Result Pattern, Messaging) |
| 🏗️ Arquitetura | [API-GATEWAY.md](../architecture/API-GATEWAY.md) | Gateway que roteia chamadas para este serviço |
| 📊 Review Consolidado | [LIFESYNC_CODE_REVIEW_CONSOLIDADO.md](../code-reviews/LIFESYNC_CODE_REVIEW_CONSOLIDADO.md) | Visão consolidada de todos os serviços |

[← Voltar ao Índice de Documentação](../README.md)
| 11 | Padronizar tipos de ID para `int` em todos os DTOs | DTOs | 1h |
| 12 | Implementar paginação com `Skip().Take()` | `UserService.cs` | 1h |
| 13 | Adicionar rate limiting | `Program.cs` | 1h |

### Prioridade 3 — Médios (Esforço Total: ~8h)

| # | Recomendação | Arquivo | Tempo |
|---|---|---|---|
| 14 | Melhorar regex de email (`Contact`) | `Contact.cs` | 15 min |
| 15 | Adicionar validação FluentValidation nos DTOs | DTOs | 2h |
| 16 | Renomear `LoginDTO.UserNameOrEmail` para `Email` | `LoginDTO.cs` | 15 min |
| 17 | Implementar Outbox Pattern no SignUp | `SignUpCommandHandler` | 4h |
| 18 | Remover token do body do email de reset | `EmailService.cs` | 15 min |
| 19 | Configurar CORS restritivo | `Program.cs` | 30 min |
| 20 | Implementar MFA (opcional mas recomendado) | — | 8h+ |

---

## Score / Qualidade

| Dimensão | Nota | Observações |
|---|---|---|
| **Segurança Geral** | 3/10 | Senha fraca, sem blacklist, CORS permissivo, sem rate limiting |
| **Arquitetura** | 7/10 | Good separation, Value Objects, Domain Events, mas faltam transações |
| **Código** | 5/10 | N+1, bug no UpdateUser, handler vazio, inconsistência de tipos |
| **API/Contrato** | 6/10 | Estrutura clara, mas sem paginação e DTOs sem validação |
| **Compliance** | 4/10 | OWASP non-compliant, sem HTTPS enforcement |

### Nota Geral: **5/10**

> **Atenção:** Por ser o serviço de autenticação, os problemas de segurança têm peso elevado. As 6 issues críticas representam riscos reais e devem ser tratadas com urgência.

---

## Problemas Conhecidos

| Severidade | Problema | Descrição |
|---|---|---|
| CRÍTICO | Política de senha fraca | Mínimo 6 caracteres, apenas minúsculas obrigatórias — deveria ser 12+ com mix |
| CRÍTICO | `LastLoginAt` nunca atualizado | Campo existe mas `UpdateLastLogin()` nunca é chamado no login |
| CRÍTICO | Soft delete não filtrado | Usuários com `IsDeleted = true` ainda podem fazer login e aparecem nas listas |
| CRÍTICO | N+1 em `GetAllUsersDetailsAsync` | Carrega todos os usuários, depois faz 1 query por usuário para buscar roles |
| CRÍTICO | Bug no `UsersController.UpdateUser` | Cria `updateCommand` com o userId correto mas envia o `command` original — variável nunca usada |
| CRÍTICO | `DeleteUserCommandHandler` vazio | Handler não implementado — operação de delete não funciona |
| ALTO | Sem token blacklist | Access token permanece válido até expirar mesmo após logout |
| ALTO | E-mail alterado sem verificação | Mudança de e-mail não exige confirmação |
| ALTO | Token exposto no e-mail de reset | Token de redefinição de senha exibido no corpo do e-mail HTML |
| ALTO | `UserDTO.Id` é `string`, `UpdateUserDTO.Id` é `int` | Inconsistência de tipo de ID entre DTOs |
| ALTO | Sem paginação | `GetAll` retorna todos os usuários sem limite |
| ALTO | CORS permissivo | `AllowAnyMethod()` e `AllowAnyHeader()` sem restrição |
| ALTO | Refresh token expiry hardcoded | 7 dias fixo no `AuthService`, ignora `JwtSettings.RefreshTokenExpiryDays` |
| MÉDIO | Regex de e-mail permissivo | `test@c` é considerado válido pelo regex do `Contact` |
| MÉDIO | DTOs sem validação | Nenhum DTO possui FluentValidation ou DataAnnotations |
| MÉDIO | `LoginDTO.UserNameOrEmail` enganoso | Propriedade aceita apenas e-mail, não username |

---

## Configuração

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=LifeSync;Include Error Detail=true;"
  },
  "JwtSettings": {
    "Key": "SuperSecretKeyForJWTAuthentication2024!@#$%",
    "Issuer": "LifeSyncAPI",
    "Audience": "LifeSyncApp",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "SmtpSettings": {
    "Host": "localhost",
    "Port": 1025,
    "From": "no-reply@test.local",
    "EnableSsl": false
  },
  "RabbitMQSettings": {
    "Host": "rabbitmq",
    "User": "guest",
    "Password": "guest",
    "Port": 5672
  }
}
```

---

## Dependências

### Pacotes NuGet

| Pacote | Versão | Uso |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 10.0.1 | Autenticação JWT |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | 10.0.1 | Identity + EF Core |
| `Microsoft.EntityFrameworkCore` | 10.0.1 | ORM |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.0 | Provider PostgreSQL |
| `Swashbuckle.AspNetCore` | 10.1.0 | Swagger/OpenAPI |
| `MediatR` | 14.0.0 | CQRS dispatcher |

### Referências Internas

| Projeto | Uso |
|---|---|
| `BuildingBlocks` | CQRS, Result, Validation, JWT |
| `BuildingBlocks.Messaging` | RabbitMQ (publicar `UserRegisteredEvent`) |
| `Core.Domain` | BaseEntity, ValueObject, IDomainEvent |
| `Core.API` | ApiController base |

### RabbitMQ — Eventos Publicados

| Exchange | Routing Key | Evento |
|---|---|---|
| `user_exchange` | `user.registered` | `UserRegisteredEvent` |
