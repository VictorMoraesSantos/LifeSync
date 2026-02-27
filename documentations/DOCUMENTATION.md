# 📚 Documentação Completa - Microserviço de Autenticação e Usuários (Users Service)

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Arquitetura](#arquitetura)
3. [Tecnologias](#tecnologias)
4. [Configuração](#configuração)
5. [Endpoints da API](#endpoints-da-api)
6. [DTOs (Data Transfer Objects)](#dtos-data-transfer-objects)
7. [Entidades de Domínio](#entidades-de-domínio)
8. [Commands e Handlers](#commands-e-handlers)
9. [Validações](#validações)
10. [Autenticação e Segurança](#autenticação-e-segurança)
11. [Tratamento de Erros](#tratamento-de-erros)
12. [Exemplos de Uso](#exemplos-de-uso)

---

## 🎯 Visão Geral

O **Users Service** é o microserviço responsável pela autenticação, autorização e gerenciamento de usuários no ecossistema LifeSync. Ele fornece todas as funcionalidades relacionadas a:

- ✅ Registro de novos usuários (Sign Up)
- ✅ Autenticação (Login/Logout)
- ✅ Recuperação de senha (Forgot Password / Reset Password)
- ✅ Alteração de senha (Change Password)
- ✅ Confirmação de email
- ✅ Gerenciamento de perfil de usuário
- ✅ Geração e validação de tokens JWT
- ✅ Refresh Tokens para renovação de sessões

---

## 🏗️ Arquitetura

O microserviço segue a **Clean Architecture** com separação em camadas:

```
Users.API/              # Camada de Apresentação (Controllers, Endpoints)
Users.Application/      # Camada de Aplicação (Commands, Queries, DTOs)
Users.Domain/           # Camada de Domínio (Entidades, Value Objects, Events)
Users.Infrastructure/   # Camada de Infraestrutura (Repositórios, Contexto DB)
```

### Padrões Utilizados

- **CQRS (Command Query Responsibility Segregation)**: Separação entre comandos (write) e queries (read)
- **Mediator Pattern**: Usando MediatR para desacoplar requisições dos handlers
- **Repository Pattern**: Abstração do acesso a dados
- **Domain Events**: Eventos de domínio para ações críticas
- **Value Objects**: Objetos imutáveis para encapsular lógica de negócio
- **Result Pattern**: Retorno padronizado de sucesso/erro sem exceptions

---

## 🛠️ Tecnologias

### Framework e Linguagem
- **.NET 9.0**
- **C# 13**
- **ASP.NET Core Web API**

### Bibliotecas Principais
- **Microsoft.AspNetCore.Identity** - Sistema de identidade e autenticação
- **Entity Framework Core 9.0** - ORM para acesso a dados
- **MediatR** - Implementação do padrão Mediator
- **FluentValidation** - Validação de comandos e DTOs
- **AutoMapper** - Mapeamento entre entidades e DTOs

### Segurança
- **JWT (JSON Web Tokens)** - Autenticação stateless
- **BCrypt** - Hash de senhas (via Identity)
- **Refresh Tokens** - Renovação segura de tokens

### Banco de Dados
- **PostgreSQL** - Banco de dados relacional
- **EF Core Migrations** - Controle de versão do schema

### Mensageria
- **RabbitMQ** - Message broker para comunicação assíncrona

---

## ⚙️ Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=LifeSync;"
  },
  "JwtSettings": {
    "Key": "SuperSecretKeyForJWTAuthentication2024!@#$%",
    "Issuer": "LifeSyncAPI",
    "Audience": "LifeSyncApp",
    "ExpiryMinutes": 60
  },
  "SmtpSettings": {
    "Host": "localhost",
    "Port": 1025,
    "User": "",
    "Password": "",
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

### Variáveis de Ambiente (Docker)

```env
CONNECTIONSTRINGS__DATABASE=Server=postgres;Port=5432;Database=LifeSync;User Id=postgres;Password=postgres;
JWTSETTINGS__KEY=YourSuperSecretKey
JWTSETTINGS__ISSUER=LifeSyncAPI
JWTSETTINGS__AUDIENCE=LifeSyncApp
JWTSETTINGS__EXPIRYMINUTES=60
```

---

## 🌐 Endpoints da API

### Base URL
```
http://localhost:5001/api/auth
http://localhost:5001/api/users
```

---

### 🔐 AuthController

#### 1. Login
**Endpoint:** `POST /api/auth/login`  
**Autenticação:** Não requerida ([AllowAnonymous])  
**Descrição:** Autentica um usuário e retorna tokens de acesso.

**Request Body:**
```json
{
  "email": "usuario@exemplo.com",
  "password": "Senha@123"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0...",
  "user": {
    "id": 1,
    "firstName": "João",
    "lastName": "Silva",
    "email": "usuario@exemplo.com",
    "isEmailConfirmed": true,
    "birthDate": "1990-05-15",
    "createdAt": "2024-01-15T10:30:00Z",
    "lastLoginAt": "2024-02-10T01:45:00Z"
  }
}
```

**Response (400 Bad Request):**
```json
{
  "error": "Email ou senha inválidos"
}
```

**Possíveis Erros:**
- ❌ Email não encontrado
- ❌ Senha incorreta
- ❌ Conta não confirmada
- ❌ Conta desativada

---

#### 2. Register (Sign Up)
**Endpoint:** `POST /api/auth/register`  
**Autenticação:** Não requerida ([AllowAnonymous])  
**Descrição:** Registra um novo usuário no sistema.

**Request Body:**
```json
{
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao.silva@exemplo.com",
  "password": "Senha@123",
  "confirmPassword": "Senha@123",
  "birthDate": "1990-05-15"
}
```

**Response (200 OK):**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0...",
  "user": {
    "id": 1,
    "firstName": "João",
    "lastName": "Silva",
    "email": "joao.silva@exemplo.com",
    "isEmailConfirmed": false,
    "birthDate": "1990-05-15",
    "createdAt": "2024-02-10T01:45:00Z"
  }
}
```

**Response (400 Bad Request):**
```json
{
  "error": "Email já está em uso"
}
```

**Validações:**
- ✅ Email deve ser válido e único
- ✅ Senha deve ter no mínimo 8 caracteres
- ✅ Senha deve conter: maiúsculas, minúsculas, números e caracteres especiais
- ✅ ConfirmPassword deve ser igual a Password
- ✅ FirstName e LastName são obrigatórios
- ✅ BirthDate é opcional, mas se fornecido, deve ser uma data válida

---

#### 3. Logout
**Endpoint:** `POST /api/auth/logout`  
**Autenticação:** Requerida (Bearer Token)  
**Descrição:** Invalida o refresh token do usuário.

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request Body:** Vazio

**Response (200 OK):**
```json
{
  "message": "Logout realizado com sucesso"
}
```

**Response (401 Unauthorized):**
```json
{
  "error": "Token inválido ou expirado"
}
```

---

#### 4. Send Email Confirmation
**Endpoint:** `POST /api/auth/send-email-confirmation`  
**Autenticação:** Requerida  
**Descrição:** Envia um email de confirmação para o usuário.

**Request Body:**
```json
{
  "email": "usuario@exemplo.com"
}
```

**Response (200 OK):**
```json
{
  "message": "Email de confirmação enviado com sucesso"
}
```

---

#### 5. Forgot Password
**Endpoint:** `POST /api/auth/forgot-password`  
**Autenticação:** Não requerida  
**Descrição:** Envia um email com token para redefinição de senha.

**Request Body:**
```json
{
  "email": "usuario@exemplo.com"
}
```

**Response (200 OK):**
```json
{
  "message": "Email de recuperação enviado com sucesso"
}
```

**Observações:**
- ⚠️ Sempre retorna sucesso, mesmo se o email não existir (segurança)
- 📧 Email contém um token válido por 1 hora

---

#### 6. Reset Password
**Endpoint:** `POST /api/auth/reset-password`  
**Autenticação:** Não requerida  
**Descrição:** Redefine a senha usando o token recebido por email.

**Request Body:**
```json
{
  "email": "usuario@exemplo.com",
  "token": "CfDJ8KbO...",
  "newPassword": "NovaSenha@123",
  "confirmPassword": "NovaSenha@123"
}
```

**Response (200 OK):**
```json
{
  "message": "Senha redefinida com sucesso"
}
```

**Response (400 Bad Request):**
```json
{
  "error": "Token inválido ou expirado"
}
```

---

#### 7. Change Password
**Endpoint:** `POST /api/auth/change-password`  
**Autenticação:** Requerida  
**Descrição:** Altera a senha do usuário autenticado.

**Headers:**
```
Authorization: Bearer {accessToken}
```

**Request Body:**
```json
{
  "currentPassword": "SenhaAtual@123",
  "newPassword": "NovaSenha@123",
  "confirmPassword": "NovaSenha@123"
}
```

**Response (200 OK):**
```json
{
  "message": "Senha alterada com sucesso"
}
```

**Response (400 Bad Request):**
```json
{
  "error": "Senha atual incorreta"
}
```

---

### 👤 UsersController

#### 1. Get User by ID
**Endpoint:** `GET /api/users/{userId}`  
**Autenticação:** Requerida  
**Descrição:** Retorna os dados de um usuário específico.

**Response (200 OK):**
```json
{
  "id": 1,
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao.silva@exemplo.com",
  "phoneNumber": "+55 11 98765-4321",
  "isEmailConfirmed": true,
  "birthDate": "1990-05-15",
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-02-10T01:45:00Z",
  "lastLoginAt": "2024-02-10T01:45:00Z",
  "isActive": true
}
```

**Response (404 Not Found):**
```json
{
  "error": "Usuário não encontrado"
}
```

---

#### 2. Get All Users
**Endpoint:** `GET /api/users`  
**Autenticação:** Requerida (Admin)  
**Descrição:** Retorna lista de todos os usuários.

**Response (200 OK):**
```json
[
  {
    "id": 1,
    "firstName": "João",
    "lastName": "Silva",
    "email": "joao.silva@exemplo.com",
    "isActive": true
  },
  {
    "id": 2,
    "firstName": "Maria",
    "lastName": "Santos",
    "email": "maria.santos@exemplo.com",
    "isActive": true
  }
]
```

---

#### 3. Update User
**Endpoint:** `PUT /api/users/{userId}`  
**Autenticação:** Requerida  
**Descrição:** Atualiza os dados do perfil do usuário.

**Request Body:**
```json
{
  "firstName": "João Pedro",
  "lastName": "Silva Santos",
  "email": "joao.pedro@exemplo.com",
  "birthDate": "1990-05-15"
}
```

**Response (200 OK):**
```json
{
  "message": "Usuário atualizado com sucesso"
}
```

**Response (400 Bad Request):**
```json
{
  "error": "Não foi possível atualizar o usuário"
}
```

---

## 📦 DTOs (Data Transfer Objects)

### AuthResult
```csharp
public record AuthResult(
    string AccessToken,
    string RefreshToken,
    UserDTO User
);
```

**Descrição:** Retornado após login ou registro bem-sucedido.

**Propriedades:**
- `AccessToken` (string): Token JWT para autenticação (válido por 60 minutos)
- `RefreshToken` (string): Token para renovação do access token (válido por 7 dias)
- `User` (UserDTO): Dados do usuário autenticado

---

### LoginDTO
```csharp
public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}
```

**Validações:**
- `Email`: Obrigatório, formato de email válido
- `Password`: Obrigatório, mínimo 8 caracteres

---

### SignUpRequest
```csharp
public class SignUpRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public DateOnly? BirthDate { get; set; }
}
```

**Validações:**
- `FirstName`: Obrigatório, 2-50 caracteres
- `LastName`: Obrigatório, 2-50 caracteres
- `Email`: Obrigatório, formato válido, único no sistema
- `Password`: Obrigatório, mínimo 8 caracteres, deve conter:
  - Pelo menos 1 letra maiúscula
  - Pelo menos 1 letra minúscula
  - Pelo menos 1 número
  - Pelo menos 1 caractere especial (@, #, $, %, etc.)
- `ConfirmPassword`: Deve ser igual a `Password`
- `BirthDate`: Opcional, deve ser data passada

---

### ChangePasswordDTO
```csharp
public class ChangePasswordDTO
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
```

**Validações:**
- `CurrentPassword`: Obrigatório
- `NewPassword`: Obrigatório, mesmas regras de `Password` do SignUp
- `ConfirmPassword`: Deve ser igual a `NewPassword`
- `NewPassword`: Não pode ser igual a `CurrentPassword`

---

### ForgotPasswordDTO
```csharp
public class ForgotPasswordDTO
{
    public string Email { get; set; }
}
```

**Validações:**
- `Email`: Obrigatório, formato válido

---

### ResetPasswordDTO
```csharp
public class ResetPasswordDTO
{
    public string Email { get; set; }
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
}
```

**Validações:**
- `Email`: Obrigatório, formato válido
- `Token`: Obrigatório
- `NewPassword`: Obrigatório, mesmas regras de senha
- `ConfirmPassword`: Deve ser igual a `NewPassword`

---

### UserDTO
```csharp
public class UserDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public DateOnly? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}
```

**Descrição:** Representa os dados do usuário sem informações sensíveis.

---

## 🏛️ Entidades de Domínio

### User (Entity)

```csharp
public class User : IdentityUser<int>, IBaseEntity<int>
{
    // Value Objects
    public Name Name { get; private set; }
    public Contact Contact { get; private set; }
    
    // Propriedades
    public DateOnly? BirthDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsActive { get; private set; }
    
    // Refresh Token
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    
    // Domain Events
    public IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    
    // Métodos
    public void UpdateProfile(Name name, Contact contact)
    public void Deactivate()
    public void Activate()
    public void UpdateLastLogin()
    public void MarkAsUpdated()
    public void MarkAsDeleted()
}
```

**Herda de:** `IdentityUser<int>` (Microsoft.AspNetCore.Identity)

**Propriedades Herdadas:**
- `Id` (int): Identificador único
- `UserName` (string): Nome de usuário (usado como email)
- `Email` (string): Email do usuário
- `EmailConfirmed` (bool): Se o email foi confirmado
- `PasswordHash` (string): Hash da senha
- `PhoneNumber` (string?): Telefone (opcional)
- `TwoFactorEnabled` (bool): Se 2FA está habilitado
- `LockoutEnd` (DateTimeOffset?): Fim do bloqueio temporário
- `AccessFailedCount` (int): Contador de tentativas de login falhadas

---

### Value Objects

#### Name (Value Object)
```csharp
public record Name
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string FullName => $"{FirstName} {LastName}";
    
    public Name(string firstName, string lastName)
    {
        // Validações
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required");
            
        FirstName = firstName;
        LastName = lastName;
    }
}
```

#### Contact (Value Object)
```csharp
public record Contact
{
    public string Email { get; init; }
    public string? PhoneNumber { get; init; }
    
    public Contact(string email, string? phoneNumber = null)
    {
        // Validação de email
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format");
            
        Email = email;
        PhoneNumber = phoneNumber;
    }
    
    private static bool IsValidEmail(string email) { /* ... */ }
}
```

---

## ⚡ Commands e Handlers

### Padrão CQRS

Todos os comandos seguem o padrão:
```csharp
public record XxxCommand(...) : ICommand<Result>;
```

Todos os handlers seguem:
```csharp
public class XxxCommandHandler : ICommandHandler<XxxCommand, Result>
{
    public async Task<Result> Handle(XxxCommand command, CancellationToken cancellationToken)
    {
        // Lógica de negócio
    }
}
```

---

### LoginCommand

**Comando:**
```csharp
public record LoginCommand(string Email, string Password) : ICommand<AuthResult>;
```

**Handler:** `LoginCommandHandler`

**Fluxo:**
1. Buscar usuário por email
2. Verificar se usuário existe
3. Verificar se a conta está ativa
4. Validar senha usando `SignInManager.CheckPasswordSignInAsync`
5. Atualizar `LastLoginAt`
6. Gerar Access Token (JWT)
7. Gerar Refresh Token
8. Salvar Refresh Token no banco
9. Retornar `AuthResult`

**Dependências:**
- `UserManager<User>`
- `SignInManager<User>`
- `ITokenService`
- `IUserRepository`

---

### SignUpCommand

**Comando:**
```csharp
public record SignUpCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    DateOnly? BirthDate
) : ICommand<AuthResult>;
```

**Handler:** `SignUpCommandHandler`

**Fluxo:**
1. Validar dados de entrada
2. Verificar se email já existe
3. Criar Value Objects (Name, Contact)
4. Criar entidade User
5. Criar usuário usando `UserManager.CreateAsync`
6. Atribuir role "User" padrão
7. Gerar token de confirmação de email
8. Enviar email de confirmação (assíncrono via RabbitMQ)
9. Gerar tokens (Access e Refresh)
10. Retornar `AuthResult`

**Domain Events Disparados:**
- `UserCreatedEvent`

---

### ChangePasswordCommand

**Comando:**
```csharp
public record ChangePasswordCommand(
    ClaimsPrincipal User,
    string CurrentPassword,
    string NewPassword
) : ICommand<Result>;
```

**Handler:** `ChangePasswordCommandHandler`

**Fluxo:**
1. Obter userId do ClaimsPrincipal
2. Buscar usuário
3. Validar senha atual
4. Verificar se nova senha é diferente da atual
5. Alterar senha usando `UserManager.ChangePasswordAsync`
6. Invalidar todos os refresh tokens (forçar re-login)
7. Retornar sucesso

---

### ForgotPasswordCommand

**Comando:**
```csharp
public record ForgotPasswordCommand(string Email) : ICommand<Result>;
```

**Handler:** `ForgotPasswordCommandHandler`

**Fluxo:**
1. Buscar usuário por email (silenciosamente, não revelar se existe)
2. Se usuário existe:
   - Gerar token de reset usando `UserManager.GeneratePasswordResetTokenAsync`
   - Enviar email com link de reset (assíncrono via RabbitMQ)
3. Sempre retornar sucesso (segurança)

**Observações:**
- ⏱️ Token válido por 1 hora
- 🔒 Não revela se email existe no sistema

---

### ResetPasswordCommand

**Comando:**
```csharp
public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword
) : ICommand<Result>;
```

**Handler:** `ResetPasswordCommandHandler`

**Fluxo:**
1. Buscar usuário por email
2. Validar token usando `UserManager.VerifyUserTokenAsync`
3. Resetar senha usando `UserManager.ResetPasswordAsync`
4. Invalidar todos os refresh tokens
5. Enviar email de confirmação de alteração
6. Retornar sucesso

---

## ✅ Validações

### FluentValidation

Todas as validações são feitas usando **FluentValidation** em validators específicos:

```csharp
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");
            
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres");
    }
}
```

### Regras de Senha

Configuradas no `IdentityOptions`:

```csharp
options.Password.RequireDigit = true;              // Requer número
options.Password.RequiredLength = 8;               // Mínimo 8 caracteres
options.Password.RequireNonAlphanumeric = true;    // Requer caractere especial
options.Password.RequireUppercase = true;          // Requer maiúscula
options.Password.RequireLowercase = true;          // Requer minúscula
```

### Regras de Lockout

```csharp
options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
options.Lockout.MaxFailedAccessAttempts = 5;
options.Lockout.AllowedForNewUsers = true;
```

**Comportamento:**
- Após 5 tentativas de login falhadas, conta é bloqueada por 15 minutos
- Contador é resetado após login bem-sucedido

---

## 🔐 Autenticação e Segurança

### JWT (JSON Web Tokens)

#### Estrutura do Access Token

**Header:**
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

**Payload (Claims):**
```json
{
  "sub": "1",
  "email": "usuario@exemplo.com",
  "unique_name": "usuario@exemplo.com",
  "nameid": "1",
  "role": "User",
  "nbf": 1707530400,
  "exp": 1707534000,
  "iat": 1707530400,
  "iss": "LifeSyncAPI",
  "aud": "LifeSyncApp"
}
```

**Claims Personalizadas:**
- `sub`: User ID
- `email`: Email do usuário
- `role`: Roles do usuário (User, Admin, etc.)
- `nameid`: User ID (compatibilidade ASP.NET Identity)

**Validade:**
- Access Token: **60 minutos**
- Refresh Token: **7 dias**

---

### Refresh Token Flow

```
1. Cliente faz login
   ↓
2. Servidor retorna Access Token + Refresh Token
   ↓
3. Cliente usa Access Token em requisições
   ↓
4. Access Token expira
   ↓
5. Cliente envia Refresh Token para /api/auth/refresh
   ↓
6. Servidor valida Refresh Token
   ↓
7. Servidor retorna novo Access Token + novo Refresh Token
   ↓
8. Cliente usa novo Access Token
```

**Segurança:**
- Refresh Token é armazenado no banco de dados
- Apenas um Refresh Token ativo por usuário
- Refresh Token é invalidado ao fazer logout
- Refresh Token é invalidado ao mudar senha

---

### Proteção de Endpoints

**Público (AllowAnonymous):**
- `POST /api/auth/login`
- `POST /api/auth/register`
- `POST /api/auth/forgot-password`
- `POST /api/auth/reset-password`

**Autenticado (Authorize):**
- `POST /api/auth/logout`
- `POST /api/auth/change-password`
- `POST /api/auth/send-email-confirmation`
- `GET /api/users/{userId}`
- `PUT /api/users/{userId}`

**Admin (Authorize Roles = "Admin"):**
- `GET /api/users`

---

### HTTPS e CORS

**HTTPS:**
- ✅ Obrigatório em produção
- ⚠️ Desenvolvimento pode usar HTTP (localhost)

**CORS:**
```csharp
services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://lifesync.app")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});
```

---

## ❌ Tratamento de Erros

### Result Pattern

Todos os handlers retornam `Result<T>` ao invés de lançar exceptions:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public Error? Error { get; }
    
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
}
```

**Benefícios:**
- ✅ Fluxo explícito de sucesso/erro
- ✅ Sem try-catch em toda a aplicação
- ✅ Melhor performance (sem stack unwinding)
- ✅ Erros tipados e previsíveis

---

### Códigos HTTP

| Código | Descrição | Quando Usar |
|--------|-----------|-------------|
| 200 OK | Sucesso | Operação bem-sucedida (GET, POST, PUT) |
| 201 Created | Criado | Recurso criado (POST /register) |
| 204 No Content | Sem conteúdo | Operação bem-sucedida sem retorno |
| 400 Bad Request | Requisição inválida | Validação falhou, dados incorretos |
| 401 Unauthorized | Não autorizado | Token ausente ou inválido |
| 403 Forbidden | Proibido | Token válido mas sem permissão |
| 404 Not Found | Não encontrado | Recurso não existe |
| 409 Conflict | Conflito | Email já existe, recurso duplicado |
| 500 Internal Server Error | Erro interno | Erro inesperado no servidor |

---

### Mensagens de Erro Padronizadas

```json
{
  "error": "Descrição legível do erro",
  "code": "ERROR_CODE",
  "timestamp": "2024-02-10T01:45:00Z",
  "path": "/api/auth/login"
}
```

**Exemplos de Códigos:**
- `USER_NOT_FOUND`: Usuário não encontrado
- `INVALID_CREDENTIALS`: Credenciais inválidas
- `EMAIL_ALREADY_EXISTS`: Email já cadastrado
- `INVALID_TOKEN`: Token inválido ou expirado
- `ACCOUNT_LOCKED`: Conta bloqueada temporariamente
- `EMAIL_NOT_CONFIRMED`: Email não confirmado

---

## 📝 Exemplos de Uso

### Exemplo 1: Fluxo Completo de Registro

```bash
# 1. Registrar novo usuário
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "João",
    "lastName": "Silva",
    "email": "joao@exemplo.com",
    "password": "Senha@123",
    "confirmPassword": "Senha@123",
    "birthDate": "1990-05-15"
  }'

# Response:
{
  "accessToken": "eyJhbG...",
  "refreshToken": "abc123...",
  "user": {
    "id": 1,
    "firstName": "João",
    "lastName": "Silva",
    "email": "joao@exemplo.com",
    "isEmailConfirmed": false
  }
}

# 2. Confirmar email (usuário clica no link do email)
curl -X POST http://localhost:5001/api/auth/confirm-email \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@exemplo.com",
    "token": "CfDJ8KbO..."
  }'
```

---

### Exemplo 2: Login e Acesso a Recurso Protegido

```bash
# 1. Fazer login
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@exemplo.com",
    "password": "Senha@123"
  }'

# Response:
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "a1b2c3d4e5f6g7h8i9j0...",
  "user": { ... }
}

# 2. Acessar recurso protegido
curl -X GET http://localhost:5001/api/users/1 \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

# Response:
{
  "id": 1,
  "firstName": "João",
  "lastName": "Silva",
  "email": "joao@exemplo.com",
  ...
}
```

---

### Exemplo 3: Recuperação de Senha

```bash
# 1. Solicitar reset de senha
curl -X POST http://localhost:5001/api/auth/forgot-password \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@exemplo.com"
  }'

# Response:
{
  "message": "Email de recuperação enviado com sucesso"
}

# 2. Usuário recebe email com token e reseta a senha
curl -X POST http://localhost:5001/api/auth/reset-password \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@exemplo.com",
    "token": "CfDJ8KbO...",
    "newPassword": "NovaSenha@456",
    "confirmPassword": "NovaSenha@456"
  }'

# Response:
{
  "message": "Senha redefinida com sucesso"
}
```

---

### Exemplo 4: Refresh Token

```bash
# Quando o Access Token expira, renovar usando Refresh Token
curl -X POST http://localhost:5001/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
    "accessToken": "eyJhbG... (token expirado)",
    "refreshToken": "a1b2c3d4e5f6g7h8i9j0..."
  }'

# Response:
{
  "accessToken": "eyJhbG... (novo token)",
  "refreshToken": "z9y8x7w6v5u4t3s2r1... (novo refresh token)"
}
```

---

### Exemplo 5: Alterar Senha (Usuário Autenticado)

```bash
curl -X POST http://localhost:5001/api/auth/change-password \
  -H "Authorization: Bearer eyJhbG..." \
  -H "Content-Type: application/json" \
  -d '{
    "currentPassword": "Senha@123",
    "newPassword": "NovaSenha@789",
    "confirmPassword": "NovaSenha@789"
  }'

# Response:
{
  "message": "Senha alterada com sucesso"
}
```

---

### Exemplo 6: Atualizar Perfil

```bash
curl -X PUT http://localhost:5001/api/users/1 \
  -H "Authorization: Bearer eyJhbG..." \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "João Pedro",
    "lastName": "Silva Santos",
    "email": "joao.pedro@exemplo.com",
    "birthDate": "1990-05-15"
  }'

# Response:
{
  "message": "Usuário atualizado com sucesso"
}
```

---

## 🧪 Testando com cURL

### Variáveis de Ambiente

```bash
# Definir URL base
export API_URL="http://localhost:5001/api"

# Fazer login e salvar token
TOKEN=$(curl -s -X POST $API_URL/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"joao@exemplo.com","password":"Senha@123"}' \
  | jq -r '.accessToken')

# Usar token em requisições
curl -X GET $API_URL/users/1 \
  -H "Authorization: Bearer $TOKEN"
```

---

## 📊 Diagramas

### Fluxo de Autenticação (Login)

```
┌─────────┐          ┌──────────┐          ┌──────────┐          ┌──────────┐
│ Cliente │          │   API    │          │ Identity │          │ Database │
└────┬────┘          └─────┬────┘          └─────┬────┘          └─────┬────┘
     │                     │                     │                     │
     │  POST /auth/login   │                     │                     │
     │────────────────────>│                     │                     │
     │                     │                     │                     │
     │                     │  Buscar usuário     │                     │
     │                     │────────────────────>│                     │
     │                     │                     │   SELECT User       │
     │                     │                     │────────────────────>│
     │                     │                     │<────────────────────│
     │                     │<────────────────────│                     │
     │                     │                     │                     │
     │                     │  Validar senha      │                     │
     │                     │────────────────────>│                     │
     │                     │<────────────────────│                     │
     │                     │                     │                     │
     │                     │  Gerar JWT          │                     │
     │                     │─────────────┐       │                     │
     │                     │             │       │                     │
     │                     │<────────────┘       │                     │
     │                     │                     │                     │
     │                     │  Gerar Refresh      │                     │
     │                     │─────────────┐       │                     │
     │                     │             │       │                     │
     │                     │<────────────┘       │                     │
     │                     │                     │                     │
     │                     │  Salvar Refresh     │                     │
     │                     │────────────────────>│                     │
     │                     │                     │   UPDATE User       │
     │                     │                     │────────────────────>│
     │                     │                     │<────────────────────│
     │                     │<────────────────────│                     │
     │                     │                     │                     │
     │  200 OK + Tokens    │                     │                     │
     │<────────────────────│                     │                     │
     │                     │                     │                     │
```

---

## 🔗 Integração com Outros Serviços

### RabbitMQ (Mensageria)

**Events Publicados:**

1. **UserCreatedEvent**
   - Exchange: `user.events`
   - Routing Key: `user.created`
   - Payload: `{ userId, email, firstName, lastName }`
   - Consumidores:
     - Notification Service (envia email de boas-vindas)

2. **EmailConfirmationRequestedEvent**
   - Exchange: `user.events`
   - Routing Key: `email.confirmation.requested`
   - Payload: `{ userId, email, confirmationToken }`
   - Consumidores:
     - Notification Service (envia email de confirmação)

3. **PasswordResetRequestedEvent**
   - Exchange: `user.events`
   - Routing Key: `password.reset.requested`
   - Payload: `{ userId, email, resetToken }`
   - Consumidores:
     - Notification Service (envia email com link de reset)

---

### API Gateway

O Users Service é acessado através do API Gateway:

```
Cliente -> API Gateway (Ocelot) -> Users Service
```

**Rotas no Gateway:**
- `/api/auth/*` → `http://users-service:5001/api/auth/*`
- `/api/users/*` → `http://users-service:5001/api/users/*`

**Responsabilidades do Gateway:**
- Rate Limiting
- Validação de Token JWT
- Logging centralizado
- Load Balancing

---

## 🚀 Executando o Serviço

### Docker

```bash
# Build da imagem
docker build -t lifesync-users:latest .

# Executar container
docker run -d \
  -p 5001:8080 \
  -e CONNECTIONSTRINGS__DATABASE="Server=postgres;..." \
  -e JWTSETTINGS__KEY="YourSecretKey" \
  --name lifesync-users \
  lifesync-users:latest
```

### Docker Compose

```bash
# Subir todos os serviços
docker-compose up -d

# Ver logs do Users Service
docker-compose logs -f users-service
```

### Desenvolvimento Local

```bash
# Restaurar pacotes
dotnet restore

# Aplicar migrations
dotnet ef database update --project Services/Users/Users.Infrastructure

# Executar
dotnet run --project Services/Users/Users.API
```

---

## 📚 Referências

- [ASP.NET Core Identity Documentation](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [JWT Best Practices](https://datatracker.ietf.org/doc/html/rfc8725)
- [OWASP Authentication Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Authentication_Cheat_Sheet.html)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)

---

## 📄 Licença

MIT License - LifeSync © 2024-2026

---

**Última Atualização:** 10 de fevereiro de 2026  
**Versão da Documentação:** 1.0.0  
**Versão da API:** v1
