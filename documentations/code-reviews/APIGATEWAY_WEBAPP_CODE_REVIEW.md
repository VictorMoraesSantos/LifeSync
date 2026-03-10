# API Gateway & WebApp - Code Review Completo

> **Autor:** Code Review Senior
> **Data:** 03/03/2026
> **Escopo:** Varredura completa do API Gateway (YARP) e WebApp (Blazor WASM)
> **Severidade:** CRITICO | ALTO | MEDIO | BAIXO | INFO

---

# PARTE 1: API GATEWAY (YARP)

## Sumario

API Gateway baseado em YARP 2.3.0 com roteamento configuration-driven, JWT authentication e path transformations. Servico bem estruturado mas com **autorizacao incompleta**, **sem rate limiting** e **sem observabilidade**.

### Nota: 6.5/10

---

## 1. ROTEAMENTO

### 1.1 [CRITICO] Autorizacao Comentada na Maioria das Rotas

**Arquivo:** `appsettings.json` (ReverseProxy section)

| Rota | Cluster | AuthorizationPolicy |
|------|---------|-------------------|
| auth-route | users-cluster | Nenhuma (publico) |
| taskmanager-route | taskmanager-cluster | **COMENTADA** |
| nutrition-route | nutrition-cluster | **COMENTADA** |
| financial-route | financial-cluster | **COMENTADA** |
| users-route | users-cluster | Nenhuma |
| gym-route | gym-cluster | RequireAuthentication |

**Impacto:** Apenas o Gym service exige autenticacao no gateway. Todos os outros endpoints sao publicos.

**Correcao:** Habilitar `"AuthorizationPolicy": "RequireAuthentication"` em todas as rotas protegidas.

---

### 1.2 [ALTO] JWT Secret Hardcoded

```json
"Key": "SuperSecretKeyForJWTAuthentication2024!@#$%"
```

Mesmo problema de todos os microservices.

---

### 1.3 [ALTO] Sem Rate Limiting

YARP suporta rate limiting nativo mas nao esta configurado. Gateway vulneravel a DoS.

**Correcao:**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("gateway", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 200;
    });
});
```

---

### 1.4 [ALTO] Sem Health Checks

Nenhum endpoint `/health` configurado. Orquestradores (Kubernetes, Docker Swarm) nao conseguem verificar se o gateway esta saudavel.

---

### 1.5 [ALTO] Sem Logging / Request Tracing

Nenhum logging de requests configurado. Sem correlation ID para rastreamento distribuido.

**Correcao:** Adicionar middleware de correlation ID e Serilog.

---

### 1.6 [MEDIO] CORS Apenas para Localhost

```csharp
policy.WithOrigins(
    "http://localhost:6007",
    "https://localhost:6067")
```

Sem configuracao para producao. Precisa de origens configuraveis via environment variables.

---

### 1.7 [MEDIO] Data Protection Keys em Volume Docker

```csharp
.PersistKeysToFileSystem(new DirectoryInfo("/keys"));
```

Funciona mas requer volume persistente no Docker. Se o volume nao for montado, chaves sao perdidas a cada restart.

---

## Plano de Acao - API Gateway

| # | Item | Severidade | Esforco |
|---|------|-----------|---------|
| 1 | Habilitar AuthorizationPolicy em todas as rotas | CRITICO | 15 min |
| 2 | Remover JWT Key do appsettings.json | ALTO | 30 min |
| 3 | Implementar rate limiting | ALTO | 1h |
| 4 | Adicionar health checks | ALTO | 30 min |
| 5 | Adicionar logging estruturado (Serilog) | ALTO | 2h |
| 6 | Adicionar correlation ID middleware | ALTO | 1h |
| 7 | CORS configuravel por environment | MEDIO | 30 min |

---

# PARTE 2: WEBAPP (BLAZOR WASM)

## Sumario

Frontend Blazor WebAssembly com Syncfusion 32.2.7, autenticacao JWT custom via localStorage, e servicos para todos os microservices. Apresenta **vulnerabilidade XSS via localStorage**, **sem refresh token**, **sem paginacao**, **error handling silencioso** e **duplicacao de modelos**.

### Nota: 5.5/10

---

## 1. AUTENTICACAO

### 1.1 [CRITICO] JWT em localStorage (Vulneravel a XSS)

**Arquivo:** `CustomAuthStateProvider.cs`

```csharp
await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
```

**Impacto:** Qualquer script malicioso (XSS) pode ler o token de localStorage.

**Mitigacao (Blazor WASM nao suporta HttpOnly cookies):**
1. Implementar Content Security Policy (CSP) rigoroso
2. Sanitizar todo input de usuario
3. Usar token de curta duracao + refresh token

---

### 1.2 [CRITICO] Sem Mecanismo de Refresh Token

O access token expira em 60 minutos. Nao ha refresh token flow implementado no frontend.

**Impacto:** Usuario e deslogado abruptamente apos 60 minutos sem aviso.

**Correcao:** Implementar refresh token flow:
```csharp
public async Task<string?> GetTokenAsync()
{
    var token = await GetStoredToken();
    if (IsExpiringSoon(token))
    {
        token = await RefreshTokenAsync();
    }
    return token;
}
```

---

### 1.3 [ALTO] Logout Nao Invalida Token no Servidor

O logout apenas remove o token do localStorage. O token continua valido no backend ate expirar.

---

### 1.4 [ALTO] Erros de Token Silenciosos

Se o token for malformado, o erro e engolido silenciosamente e o usuario vira anonimo sem notificacao.

---

## 2. API CLIENT

### 2.1 [ALTO] Sem Retry Logic

**Arquivo:** `ApiClient.cs`

Qualquer falha de rede causa erro imediato sem retry.

**Correcao:** Implementar retry com Polly:
```csharp
builder.Services.AddHttpClient<IApiClient, ApiClient>()
    .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3,
        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
```

---

### 2.2 [ALTO] Sem Timeout Configurado

Nenhum timeout no HttpClient. Requests podem travar indefinidamente.

---

### 2.3 [ALTO] Sem Circuit Breaker

Se um backend estiver fora do ar, cada request falha individualmente sem protecao.

---

### 2.4 [MEDIO] URLs Hardcoded nos Services

```csharp
private const string BaseRoute = "taskmanager-service/api/task-items";
```

**Correcao:** Centralizar URLs em configuracao.

---

## 3. COMPONENTES

### 3.1 [ALTO] Sem Paginacao nas Listagens

Todos os services carregam TODOS os registros de uma vez:
```csharp
var response = await _apiClient.GetAsync<ApiResponse<List<TaskItemDTO>>>(BaseRoute);
```

**Impacto:** Com 10.000 tarefas, toda a lista e transferida e renderizada.

**Correcao:** Implementar paginacao server-side e virtual scrolling.

---

### 3.2 [ALTO] Error Handling Silencioso nos Componentes

**Arquivo:** `Home.razor`

```csharp
try {
    var tasksResult = await TaskItemService.GetTasksByUserAsync(userId);
} catch { }  // CATCH VAZIO!
```

**Impacto:** Dashboard mostra dados vazios/incorretos sem nenhuma indicacao de erro.

**Correcao:** Implementar toast notifications para erros.

---

### 3.3 [ALTO] Filtragem Apenas Client-Side

Filtros sao aplicados em memoria no browser apos carregar todos os dados.

**Correcao:** Enviar filtros como query parameters e filtrar no servidor.

---

### 3.4 [MEDIO] Sem Loading States Visuais

`isLoading` flag existe mas nao ha skeleton/placeholder na UI. Usuario ve tela em branco durante loading.

---

### 3.5 [MEDIO] Sem Optimistic Updates

Toda operacao CRUD aguarda resposta do servidor antes de atualizar a UI.

---

### 3.6 [MEDIO] Sem Caching

Nenhuma estrategia de cache. Cada navegacao recarrega todos os dados.

---

## 4. MODELOS / DTOs

### 4.1 [MEDIO] Duplicacao de Modelos

Os DTOs do frontend sao copias manuais dos DTOs do backend. Qualquer mudanca na API requer atualizacao manual.

**Correcao (longo prazo):** Criar pacote NuGet compartilhado ou gerar DTOs automaticamente via OpenAPI.

---

### 4.2 [BAIXO] Sem Validacao Nos Modelos

DTOs nao possuem DataAnnotations. Validacao de formularios depende apenas do codigo nos componentes.

---

## 5. CONFIGURACAO

### 5.1 [MEDIO] Licenca Syncfusion Hardcoded

```json
"SyncfusionLicenseKey": "..."
```

Chave de licenca no appsettings, visivel no bundle WASM.

---

### 5.2 [BAIXO] Hack para Remover Watermark Trial

```javascript
// App.razor - JavaScript para remover watermark do Syncfusion trial
```

Solucao nao profissional. Licenca deveria ser registrada corretamente.

---

## 6. PLANO DE ACAO PRIORIZADO - WEBAPP

### Prioridade 1 - Criticos
| # | Item | Esforco |
|---|------|---------|
| 1 | Implementar Content Security Policy (CSP) | 2h |
| 2 | Implementar refresh token flow | 4h |
| 3 | Implementar paginacao server-side | 6h |

### Prioridade 2 - Altos
| # | Item | Esforco |
|---|------|---------|
| 4 | Implementar retry logic (Polly) | 2h |
| 5 | Configurar timeout no HttpClient | 30 min |
| 6 | Implementar circuit breaker | 2h |
| 7 | Implementar toast notifications para erros | 3h |
| 8 | Server-side filtering | 4h |
| 9 | Invalidar token no logout (server-side) | 2h |

### Prioridade 3 - Medios
| # | Item | Esforco |
|---|------|---------|
| 10 | Implementar loading skeletons | 3h |
| 11 | Implementar caching layer | 4h |
| 12 | Centralizar URLs de servicos | 1h |
| 13 | Pacote NuGet compartilhado para DTOs | 4h |
| 14 | Mover licenca Syncfusion para config segura | 30 min |

---

# RESUMO CONSOLIDADO

## Notas por Servico

| Servico | Nota | Issues Criticos | Issues Altos |
|---------|------|----------------|-------------|
| API Gateway | 6.5/10 | 1 | 5 |
| WebApp | 5.5/10 | 3 | 7 |

## Issues Compartilhados (Sistema Inteiro)

1. **JWT Secret identico em TODOS os servicos** - hardcoded no appsettings.json
2. **Sem observabilidade** - nenhum servico tem logging estruturado ou tracing distribuido
3. **Sem rate limiting** - nenhum servico protegido contra abuso
4. **CORS inconsistente** - cada servico configura diferente
