# Users Microservice - Code Review Completo

> **Autor:** Code Review Senior
> **Data:** 03/03/2026
> **Escopo:** Varredura completa de todas as camadas
> **Severidade:** CRITICO | ALTO | MEDIO | BAIXO | INFO

---

## Sumario Executivo

O microservice Users e o servico de autenticacao e gerenciamento de usuarios do sistema, utilizando ASP.NET Identity com JWT + Refresh Tokens. Possui Value Objects (Name, Contact) e integracao com RabbitMQ para publicacao de eventos. Apesar de ser o servico mais critico para seguranca, apresenta **politica de senha fraca**, **LastLoginAt nunca atualizado**, **soft delete nao filtrado**, **N+1 queries**, **handler de delete vazio** e **bug no controller de update**.

### Nota Geral: 5/10

---

## 1. IDENTITY / AUTENTICACAO

### 1.1 [CRITICO] Politica de Senha Fraca (OWASP Non-Compliant)

**Arquivo:** `DependencyInjection.cs`

```csharp
options.Password.RequireDigit = false;
options.Password.RequireLowercase = true;
options.Password.RequireNonAlphanumeric = false;
options.Password.RequireUppercase = false;
options.Password.RequiredLength = 6;          // Minimo 6!
options.Password.RequiredUniqueChars = 1;
```

**Impacto:** Senha "aaaaaa" e aceita. Nao atende OWASP minimum (12 chars, mixed case, special chars).

**Correcao:**
```csharp
options.Password.RequireDigit = true;
options.Password.RequireLowercase = true;
options.Password.RequireNonAlphanumeric = true;
options.Password.RequireUppercase = true;
options.Password.RequiredLength = 12;
options.Password.RequiredUniqueChars = 3;
```

---

### 1.2 [CRITICO] JWT Secret Hardcoded

```json
"Key": "SuperSecretKeyForJWTAuthentication2024!@#$%"
```

Chave JWT no `appsettings.json`, visivel no controle de versao.

---

### 1.3 [CRITICO] Sem Token Blacklist / Revocacao

O logout limpa o refresh token mas **nao invalida o access token existente**. Um token roubado continua valido ate expirar (60 minutos).

**Correcao:** Implementar token blacklist com Redis ou banco.

---

### 1.4 [ALTO] LastLoginAt Nunca Atualizado

**Arquivo:** `AuthService.LoginAsync()`

Apos login bem-sucedido, `user.UpdateLastLogin()` **nunca e chamado**. O campo `LastLoginAt` permanece null para sempre.

**Correcao:**
```csharp
user.UpdateLastLogin();
await _userManager.UpdateAsync(user);
```

---

### 1.5 [ALTO] Email Pode Ser Alterado Sem Verificacao

**Arquivo:** `UpdateUserProfileAsync()`

O email pode ser alterado diretamente sem exigir confirmacao do novo email.

**Impacto:** Risco de account takeover - atacante altera email para o proprio e depois faz password reset.

**Correcao:** Exigir verificacao de email antes de confirmar a mudanca.

---

### 1.6 [ALTO] Refresh Token Expiry Hardcoded

```csharp
user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Sem configuracao!
```

**Correcao:** Mover para `JwtSettings` configuravel.

---

### 1.7 [MEDIO] Token de Reset Exposto no Email

**Arquivo:** `EmailService.cs`

```csharp
var body = $@"
    <p><a href='{link}'>Redefinir Senha</a></p>
    <p>token: {WebUtility.HtmlEncode(message.Token ?? string.Empty)}</p>
";
```

O token de reset de senha e exibido em texto plano no corpo do email. Se o email for interceptado/logado, o token fica exposto.

**Correcao:** Remover token do body, incluir apenas no URL.

---

## 2. USER MANAGEMENT

### 2.1 [CRITICO] Soft Delete Nao Filtrado

**Arquivo:** `UserService.cs`

```csharp
var users = await _userManager.Users.AsNoTracking().ToListAsync();
```

`IsDeleted` existe no User mas NENHUMA query filtra registros deletados. Usuarios "deletados" continuam visiveis e podem fazer login.

**Correcao:** Adicionar filtro em todas as queries:
```csharp
var users = await _userManager.Users
    .Where(u => !u.IsDeleted)
    .AsNoTracking()
    .ToListAsync();
```

---

### 2.2 [ALTO] BirthDate Aceita Datas Futuras

O domain model tem `UserErrors.BirthDateInFuture` definido mas **nunca utilizado**. O construtor nao valida a data de nascimento.

**Correcao:**
```csharp
if (birthDate.HasValue && birthDate.Value > DateOnly.FromDateTime(DateTime.Today))
    throw new DomainException(UserErrors.BirthDateInFuture);
```

---

### 2.3 [ALTO] DeleteUserCommandHandler Vazio

```csharp
internal class DeleteUserCommandHandler
{
    // COMPLETAMENTE VAZIO!
}
```

A funcionalidade de deletar usuario nao esta implementada.

---

## 3. DOMAIN LAYER

### 3.1 [MEDIO] Contact Email Regex Permissiva

```csharp
private static readonly Regex EmailRegex =
    new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
```

Aceita emails como `test@invalid.c` (TLD com 1 char) e nao valida caracteres especiais.

**Correcao:**
```csharp
new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)
```

---

### 3.2 [BAIXO] Name Value Object - Edge Case

```csharp
FirstName = firstName.Trim();
```

Se `firstName` for "   " (apenas espacos), `Validate()` rejeita. Mas se for "  a  ", aceita e vira "a". Comportamento aceitavel mas nao documentado.

---

## 4. PERFORMANCE

### 4.1 [CRITICO] N+1 no GetAllUsersDetailsAsync

**Arquivo:** `UserService.cs`

```csharp
var users = await _userManager.Users.AsNoTracking().ToListAsync();
foreach (var u in users)
{
    var roles = await _userManager.GetRolesAsync(u); // N queries adicionais!
    list.Add(UserMapper.ToDto(u) with { Roles = roles.ToList() });
}
```

**Impacto:** 1000 usuarios = 1001 queries (1 para users + 1000 para roles).

**Correcao:** Fazer JOIN com UserRoles/Roles em uma unica query.

---

### 4.2 [ALTO] Sem Paginacao no GetAll

`GetAllUsersAsync()` e `GetAllUsersDetailsAsync()` retornam TODOS os usuarios de uma vez.

**Correcao:** Implementar paginacao com `Skip().Take()`.

---

## 5. INCONSISTENCIAS E CODE SMELLS

### 5.1 [CRITICO] Bug no UsersController.UpdateUser

```csharp
public async Task<HttpResult<object>> UpdateUser(
    int userId, [FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
{
    var updateCommand = new UpdateUserCommand(...); // Cria novo command com userId da route
    var result = await _sender.Send(command, cancellationToken); // MAS ENVIA O ORIGINAL!
}
```

**Impacto:** O `updateCommand` criado com o userId correto e IGNORADO. O `command` original (sem userId da route) e enviado.

**Correcao:** `await _sender.Send(updateCommand, cancellationToken);`

---

### 5.2 [ALTO] Inconsistencia de Tipos de ID

```csharp
// UserDTO usa string:
public record UserDTO(string Id, ...);

// UpdateUserDTO usa int:
public record UpdateUserDTO(int Id, ...);

// User entity usa int (IdentityUser<int>)
```

**Impacto:** Conversoes desnecessarias e possivel perda de dados.

**Correcao:** Padronizar para `int` em todo o projeto.

---

### 5.3 [MEDIO] Arquivo com Extensao Duplicada

`UpdateUserCommandHandler.cs.cs` - extensao .cs duplicada.

---

### 5.4 [MEDIO] DTOs Sem Validacao

```csharp
public class LoginDTO
{
    public string UserNameOrEmail { get; set; } // Sem [Required]!
    public string Password { get; set; }         // Sem [MinLength]!
}
```

Nenhum DTO possui DataAnnotations ou FluentValidation.

---

### 5.5 [MEDIO] LoginDTO Nome Confuso

`UserNameOrEmail` sugere que aceita username ou email, mas a implementacao so busca por email.

---

### 5.6 [MEDIO] Sem Transacao no SignUp

SignUp cria usuario e publica evento. Se a publicacao do evento falhar, o usuario ja foi criado sem notificacao.

**Correcao:** Usar Outbox Pattern ou transacao.

---

## 6. SEGURANCA ADICIONAL

### 6.1 [ALTO] Sem HTTPS Enforcement

Nao ha `app.UseHsts()` nem `app.UseHttpsRedirection()` no Program.cs.

---

### 6.2 [ALTO] CORS Permissivo

```csharp
policy.AllowAnyMethod()   // Permite DELETE, PUT, etc.
      .AllowAnyHeader()   // Permite qualquer header
```

**Correcao:** Restringir a metodos e headers necessarios.

---

### 6.3 [ALTO] Sem Rate Limiting

Endpoints de login e registro sem rate limiting. Vulneravel a brute force.

---

### 6.4 [MEDIO] Sem MFA

Nenhuma implementacao de Multi-Factor Authentication.

---

### 6.5 [MEDIO] Sem Historico de Senhas

Usuarios podem reutilizar senhas anteriores apos reset.

---

## 7. PLANO DE ACAO PRIORIZADO

### Prioridade 1 - Criticos
| # | Item | Esforco |
|---|------|---------|
| 1 | Fortalecer politica de senha (OWASP) | 15 min |
| 2 | Remover JWT Key do appsettings.json | 30 min |
| 3 | Filtrar IsDeleted em todas as queries | 1h |
| 4 | Corrigir bug no UsersController.UpdateUser | 10 min |
| 5 | Corrigir N+1 no GetAllUsersDetailsAsync | 2h |
| 6 | Implementar token blacklist/revocacao | 4h |

### Prioridade 2 - Altos
| # | Item | Esforco |
|---|------|---------|
| 7 | Atualizar LastLoginAt no login | 15 min |
| 8 | Exigir verificacao de email na mudanca | 4h |
| 9 | Implementar DeleteUserCommandHandler | 2h |
| 10 | Validar BirthDate no construtor | 15 min |
| 11 | Padronizar tipos de ID (int) | 1h |
| 12 | Implementar paginacao no GetAll | 1h |
| 13 | Adicionar rate limiting | 1h |
| 14 | Configurar HTTPS enforcement | 30 min |
| 15 | Restringir CORS | 30 min |
| 16 | Tornar refresh token expiry configuravel | 15 min |
| 17 | Remover token do body do email de reset | 15 min |

### Prioridade 3 - Medios
| # | Item | Esforco |
|---|------|---------|
| 18 | Melhorar Contact email regex | 15 min |
| 19 | Adicionar validacao nos DTOs | 2h |
| 20 | Renomear LoginDTO.UserNameOrEmail | 15 min |
| 21 | Implementar Outbox Pattern no SignUp | 4h |
| 22 | Corrigir extensao duplicada do arquivo | 5 min |
| 23 | Implementar MFA | 8h |
| 24 | Historico de senhas | 4h |

---

## Resumo Final

| Severidade | Quantidade |
|------------|-----------|
| CRITICO | 6 |
| ALTO | 11 |
| MEDIO | 7 |
| BAIXO | 1 |

**Sendo o servico de autenticacao, este e o microservice mais critico para seguranca.** Os problemas de senha fraca, token nao revogavel e soft delete nao filtrado representam riscos reais de seguranca.
