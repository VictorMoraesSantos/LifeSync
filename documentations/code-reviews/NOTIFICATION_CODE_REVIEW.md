# Notification Microservice - Code Review Completo

> **Autor:** Code Review Senior
> **Data:** 03/03/2026
> **Escopo:** Varredura completa de todas as camadas
> **Severidade:** CRITICO | ALTO | MEDIO | BAIXO | INFO

---

## Sumario Executivo

O microservice Notification e um servico event-driven que consome eventos do RabbitMQ e envia emails via SMTP. Possui Strategy Pattern para customizacao de emails. Apesar da boa intencao arquitetural, apresenta **falhas criticas de seguranca na conexao SMTP**, **ausencia total de error handling**, **consumer blocking**, **emails hardcoded** e **persistencia implementada mas nunca utilizada**.

### Nota Geral: 3.5/10

---

## 1. ARQUITETURA E PROPOSITO

### Estrutura
- **Input:** RabbitMQ events (UserRegistered, TaskDueReminder)
- **Output:** Emails via SMTP
- **Storage:** PostgreSQL (audit trail - nao utilizado)
- **REST API:** Nenhum endpoint (apenas /health)

### Observacao
E o microservice mais simples, porem o menos maduro em termos de qualidade de codigo.

---

## 2. EVENT CONSUMPTION (RabbitMQ)

### 2.1 [CRITICO] Consumer Sem Error Handling

**Arquivo:** `RabbitMqEventConsumer.cs`

```csharp
void OnMessage(string json)
{
    using var scope = _scopeFactory.CreateScope();
    var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

    var @event = (IntegrationEvent)JsonSerializer.Deserialize(
        json,
        def.EventType,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

    publisher.Publish(@event, stoppingToken).GetAwaiter().GetResult();
}
```

**Problemas:**
1. **Nenhum try-catch** - qualquer excecao crasha o consumer
2. **JSON malformado** lanca excecao nao tratada
3. **Nenhum logging** - impossivel diagnosticar falhas
4. **Sem poison message handling** - mensagens invalidas causam loop infinito

**Correcao:**
```csharp
void OnMessage(string json)
{
    try
    {
        // ... processamento
    }
    catch (JsonException ex)
    {
        _logger.LogError(ex, "Failed to deserialize message: {Json}", json);
        // Route to dead-letter queue
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to process message");
        // Retry or route to DLQ
    }
}
```

---

### 2.2 [CRITICO] Blocking Async - Thread Starvation

```csharp
publisher.Publish(@event, stoppingToken).GetAwaiter().GetResult();
```

**Impacto:** `.GetAwaiter().GetResult()` bloqueia a thread em contexto async. Com alta carga de eventos, causa thread pool starvation.

**Correcao:** Usar `await` diretamente (requer refatorar OnMessage para async).

---

### 2.3 [ALTO] Sem Dead Letter Queue (DLQ)

Mensagens que falham sao perdidas permanentemente. Nao ha DLQ configurada, nao ha retry, nao ha mecanismo de reprocessamento.

---

### 2.4 [ALTO] Sem ACK/NACK Explicito

O consumer nao mostra estrategia de acknowledgment. Se o processamento falhar, a mensagem pode ser perdida ou reprocessada infinitamente dependendo da config do RabbitMQ.

---

## 3. SMTP / EMAIL SERVICE

### 3.1 [CRITICO] Conexao SMTP Sem Criptografia

**Arquivo:** `EmailService.cs`

```csharp
await client.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.None, cancellationToken);
```

**Impacto:** `SecureSocketOptions.None` = sem TLS/SSL. Credenciais e conteudo de email trafegam em texto plano.

**Correcao:**
```csharp
await client.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.StartTls, cancellationToken);
```

---

### 3.2 [CRITICO] Nenhum Error Handling no Envio de Email

```csharp
public async Task SendEmailAsync(EmailMessageDTO dto, CancellationToken cancellationToken)
{
    // ... monta email ...
    using var client = new SmtpClient();
    await client.ConnectAsync(...);    // Pode lancar SocketException
    await client.AuthenticateAsync(...); // Pode lancar AuthenticationException
    await client.SendAsync(...);       // Pode lancar SmtpCommandException
    await client.DisconnectAsync(...);
    // NENHUM try-catch!
}
```

**Impacto:** Qualquer falha de rede, autenticacao ou SMTP crasha o handler inteiro sem logging.

---

### 3.3 [ALTO] Emails Hardcoded Sem Template

```csharp
public EmailMessageDTO CreateEmail(string eventData)
{
    return new EmailMessageDTO(
        To: eventData,
        Subject: "Welcome!",
        Body: "Thanks for registering.");
}
```

**Problemas:**
1. Conteudo hardcoded no codigo
2. Sem templates (Razor, Liquid, etc.)
3. Sem personalizacao (nome do usuario, etc.)
4. Apenas texto plano (sem HTML)
5. Nao profissional para producao

---

### 3.4 [ALTO] Sem Validacao de Email

```csharp
var dto = new EmailMessageDTO(
    To: eventData,  // Nenhuma validacao de formato!
```

`eventData` e passado como string pura. Pode conter qualquer valor, incluindo strings vazias ou invalidas.

---

### 3.5 [ALTO] Nova Conexao SMTP Por Email

```csharp
using var client = new SmtpClient();
await client.ConnectAsync(...);
// ... envia ...
await client.DisconnectAsync(...);
```

**Impacto:** Abre e fecha conexao TCP+autenticacao para cada email. Overhead significativo.

**Correcao:** Implementar connection pooling ou reutilizar conexao.

---

### 3.6 [MEDIO] Dead Code - Objeto Email Criado e Nao Usado

```csharp
var mail = new EmailMessage("no-reply@yourdomain.com", dto.To, dto.Subject, dto.Body);
// ^ Criado mas NUNCA usado! O MimeMessage e criado separadamente
```

---

## 4. PERSISTENCIA

### 4.1 [ALTO] Database Implementada Mas Nunca Utilizada

EmailMessage entity, repository e DbContext existem. Porem `EmailMessageRepository.Create()` NUNCA e chamado pelo EmailService.

**Impacto:** Database configurada, migrations executam, mas zero dados persistidos. Overhead sem valor.

**Correcao:** Persistir emails apos envio para audit trail, ou remover a persistencia.

---

### 4.2 [MEDIO] Repository com NotImplementedException

```csharp
public Task Update(EmailMessage entity, CancellationToken cancellationToken)
{
    throw new NotImplementedException();
}

public Task<int> CreateEmail(EmailMessageDTO dt, CancellationToken cancellationTokeno) // TYPO!
{
    throw new NotImplementedException();
}
```

Metodos que lancam `NotImplementedException` e parametro com typo (`cancellationTokeno`).

---

## 5. SEGURANCA

### 5.1 [CRITICO] Credenciais Hardcoded

```json
"RabbitMQSettings": {
    "User": "guest",
    "Password": "guest"
}
```

---

### 5.2 [ALTO] Sem Rate Limiting para Envio de Email

Nenhum throttling de envio. Se muitos eventos chegarem simultaneamente, pode sobrecarregar o SMTP server ou causar blacklisting.

---

### 5.3 [MEDIO] Inconsistencia de Namespaces (Refactor Incompleto)

```
EmailSender.Domain.Events    // Nome antigo
Notification.Application     // Nome novo
EmailSender.Infrastructure   // Nome antigo
```

Indica refatoracao incompleta de `EmailSender` para `Notification`.

---

## 6. QUALIDADE DE CODIGO

### 6.1 [ALTO] Zero Logging em Todo o Servico

Nenhum `ILogger` utilizado em nenhuma classe. Impossivel diagnosticar problemas em producao.

---

### 6.2 [ALTO] Zero Testes

Nenhum projeto de teste encontrado para o Notification service.

---

### 6.3 [MEDIO] Use Case com Falha Silenciosa

```csharp
public async Task HandleAsync(string eventType, string eventData, CancellationToken ct)
{
    var strategy = _strategyResolver.Resolve(eventType);
    if (strategy == null)
        return;  // FALHA SILENCIOSA - sem logging!

    var email = strategy?.CreateEmail(eventData);
    await _emailSender.SendEmailAsync(email, ct);
    // Sem error handling se SendEmailAsync falhar!
}
```

---

## 7. PLANO DE ACAO PRIORIZADO

### Prioridade 1 - Criticos
| # | Item | Esforco |
|---|------|---------|
| 1 | Adicionar try-catch no consumer | 1h |
| 2 | Corrigir blocking async (.GetAwaiter().GetResult()) | 30 min |
| 3 | Habilitar TLS no SMTP (SecureSocketOptions.StartTls) | 15 min |
| 4 | Adicionar error handling no EmailService | 1h |
| 5 | Remover credenciais do appsettings.json | 30 min |

### Prioridade 2 - Altos
| # | Item | Esforco |
|---|------|---------|
| 6 | Implementar Dead Letter Queue | 3h |
| 7 | Adicionar logging (ILogger) em todas as classes | 2h |
| 8 | Implementar templates de email (Razor ou Liquid) | 4h |
| 9 | Adicionar validacao de formato de email | 30 min |
| 10 | Implementar connection pooling SMTP | 2h |
| 11 | Persistir emails no banco (ou remover DB) | 2h |
| 12 | Criar testes unitarios e de integracao | 6h |
| 13 | Adicionar rate limiting de envio | 2h |

### Prioridade 3 - Medios
| # | Item | Esforco |
|---|------|---------|
| 14 | Remover dead code (objeto EmailMessage nao usado) | 10 min |
| 15 | Corrigir NotImplementedException nos repositories | 30 min |
| 16 | Corrigir typo cancellationTokeno | 5 min |
| 17 | Padronizar namespaces (EmailSender -> Notification) | 1h |
| 18 | Logar quando strategy nao encontrada | 15 min |
| 19 | Adicionar suporte a HTML emails | 2h |

---

## Resumo Final

| Severidade | Quantidade |
|------------|-----------|
| CRITICO | 5 |
| ALTO | 8 |
| MEDIO | 4 |
| BAIXO | 0 |

**Este e o microservice com a menor nota do sistema (3.5/10).** Requer refatoracao significativa antes de qualquer uso em producao.
