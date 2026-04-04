# API Gateway - Documentação Técnica

> **Projeto:** LifeSync  
> **Stack:** YARP 2.3.0 (Yet Another Reverse Proxy)  
> **Data da Análise:** 2026-03-03  
> **Última Atualização:** 2026-03-23

---

# 1. Visão Geral

API Gateway baseado em YARP 2.3.0 com roteamento configuration-driven, JWT authentication e path transformations. Atua como ponto de entrada único para todos os microserviços do LifeSync.

## Arquitetura de Rotas

| Rota | Cluster | Política de Autorização |
|------|---------|------------------------|
| `/auth` | users-cluster | Pública |
| `/api/taskmanager` | taskmanager-cluster | **NÃO CONFIGURADA** |
| `/api/nutrition` | nutrition-cluster | **NÃO CONFIGURADA** |
| `/api/financial` | financial-cluster | **NÃO CONFIGURADA** |
| `/api/users` | users-cluster | Pública |
| `/api/gym` | gym-cluster | `RequireAuthentication` |

---

# 2. PROBLEMAS CRÍTICOS

## 2.1 Tabela de Issues Identificadas

| ID | Severidade | Componente | Issue | Impacto | Status |
|----|------------|------------|-------|--------|--------|
| APIGW-001 | 🔴 CRÍTICO | Roteamento | AuthorizationPolicy comentada na maioria das rotas | Apenas gym-service exige autenticação; todos os outros endpoints são públicos | Aberto |
| APIGW-002 | 🔴 CRÍTICO | Segurança | JWT Secret hardcoded no appsettings.json | Chave exposta em código fonte; vulnerabilidade crítica | Aberto |
| APIGW-003 | 🟠 ALTO | Infraestrutura | Sem Rate Limiting | Gateway vulnerável a ataques DoS e abuse | Aberto |
| APIGW-004 | 🟠 ALTO | Infraestrutura | Sem Health Checks configurados | Orquestradores não conseguem verificar saúde do serviço | Aberto |
| APIGW-005 | 🟠 ALTO | Observabilidade | Sem Logging / Request Tracing | Sem correlation ID; impossível rastrear requisições distribuídas | Aberto |
| APIGW-006 | 🟠 ALTO | Segurança | Sem Correlation ID Middleware | Dificulta troubleshooting e auditoria | Aberto |
| APIGW-007 | 🟡 MÉDIO | CORS | CORS configurado apenas para localhost | Sem suporte a ambientes de produção | Aberto |
| APIGW-008 | 🟡 MÉDIO | Infraestrutura | Data Protection Keys em volume Docker | Risco de perda de chaves entre restarts | Aberto |
| APIGW-009 | 🟡 MÉDIO | Segurança | Sem rate limiting por cliente/IP | Não há proteção contra abuso por origem | Aberto |
| APIGW-010 | 🔵 BAIXO | Configuração | URLs de serviços não configuráveis via env vars | Rigidez na implantação | Aberto |

---

# 3. RECOMENDAÇÕES DE CORREÇÃO

## 3.1 Correções Críticas (Implementar Imediatamente)

### APIGW-001: Habilitar AuthorizationPolicy em Todas as Rotas

**Arquivo:** `appsettings.json`

**Antes:**
```json
"Routes": [
  {
    "RouteId": "taskmanager-route",
    "ClusterId": "taskmanager-cluster",
    "AuthorizationPolicy": null  // COMENTADO!
  }
]
```

**Depois:**
```json
"Routes": [
  {
    "RouteId": "taskmanager-route",
    "ClusterId": "taskmanager-cluster",
    "AuthorizationPolicy": "RequireAuthentication"
  }
]
```

**Esforço:** 15 minutos

---

### APIGW-002: Remover JWT Secret do Código

**Arquivo:** `appsettings.json`

**Antes:**
```json
"Jwt": {
  "Key": "SuperSecretKeyForJWTAuthentication2024!@#$%"
}
```

**Depois:**
```json
"Jwt": {
  "Key": "${JWT_SECRET_KEY}"  // Via environment variable
}
```

**Código C# (Program.cs):**
```csharp
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") 
    ?? throw new InvalidOperationException("JWT_SECRET_KEY não configurado");
```

**Esforço:** 30 minutos

---

## 3.2 Correções de Alta Prioridade

### APIGW-003: Implementar Rate Limiting

**Pacote:** `Microsoft.AspNetCore.RateLimiting`

**Implementação (Program.cs):**
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("gateway", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 200;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 10;
    });
    
    options.AddPolicy("perip", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});

app.UseRateLimiter();
```

**Esforço:** 1 hora

---

### APIGW-004: Adicionar Health Checks

**Pacotes:** `AspNetCore.HealthChecks.Yarp`

**Implementação (Program.cs):**
```csharp
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddCheck("yarp", () =>
    {
        var cluster = app.Services.GetRequiredService<IReverseProxyCache>()?
            .GetCluster("users-cluster");
        return cluster != null 
            ? HealthCheckResult.Healthy() 
            : HealthCheckResult.Unhealthy("Cluster indisponível");
    });

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready")
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("live")
});
```

**Esforço:** 30 minutos

---

### APIGW-005 & APIGW-006: Adicionar Logging Estruturado e Correlation ID

**Pacotes:**
- `Serilog.AspNetCore`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.File`

**Implementação (Program.cs):**
```csharp
app.UseHttpLogging();
app.Use(async (context, next) =>
{
    var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
        ?? Guid.NewGuid().ToString();
    
    context.Response.Headers["X-Correlation-ID"] = correlationId;
    
    using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
    using (Serilog.Context.LogContext.PushProperty("RequestPath", context.Request.Path))
    {
        await next();
    }
});
```

**appsettings.json:**
```json
"Serilog": {
  "MinimumLevel": "Information",
  "Enrich": ["FromLogContext", "WithMachineName", "WithThreadId"],
  "WriteTo": [
    { "Name": "Console" },
    {
      "Name": "File",
      "Args": {
        "path": "logs/gateway-.log",
        "rollingInterval": "Day"
      }
    }
  ]
}
```

**Esforço:** 2 horas

---

## 3.3 Correções de Média Prioridade

### APIGW-007: Configurar CORS para Produção

**Implementação (Program.cs):**
```csharp
var allowedOrigins = builder.Environment.IsDevelopment()
    ? new[] { "http://localhost:6007", "https://localhost:6067" }
    : Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',') 
      ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

**Esforço:** 30 minutos

---

### APIGW-008: Melhorar Persistência de Data Protection Keys

**Implementação (Program.cs):**
```csharp
builder.Services.AddDataProtection()
    .PersistKeysToAzureBlobStorage(new Uri("${AZURE_STORAGE_KEYS_URI}"))
    .SetApplicationName("LifeSyncGateway");
```

**Ou para Kubernetes:**
```csharp
.PersistKeysToFileSystem(new DirectoryInfo("/etc(keys"))
    .SetApplicationName("LifeSyncGateway");
```

**Esforço:** 1 hora

---

# 4. SCORE / QUALIDADE DO GATEWAY

## 4.1 Avaliação Geral

| Dimensão | Peso | Nota (0-10) | Nota Ponderada |
|----------|------|-------------|----------------|
| Segurança | 30% | 3.0 | 0.90 |
| Infraestrutura | 25% | 5.0 | 1.25 |
| Observabilidade | 20% | 2.0 | 0.40 |
| Performance | 15% | 6.0 | 0.90 |
| Configurabilidade | 10% | 5.0 | 0.50 |
| **TOTAL** | **100%** | **3.9/10** | **3.95/10** |

---

## 4.2 Detalhamento por Dimensão

### 🔒 Segurança (3.0/10)

| Critério | Status | Observação |
|----------|--------|------------|
| Autenticação nas rotas | ❌ CRÍTICO | Apenas gym-route exige auth |
| JWT Key segura | ❌ CRÍTICO | Hardcoded no código |
| Rate Limiting | ❌ Ausente | Vulnerável a DoS |
| CORS configurável | ⚠️ Parcial | Apenas localhost |
| Headers de segurança | ⚠️ Básico | Falta HSTS, CSP |

**Subnota:** 3.0/10

---

### 🏗️ Infraestrutura (5.0/10)

| Critério | Status | Observação |
|----------|--------|------------|
| Health Checks | ❌ Ausente | Sem /health endpoint |
| Data Protection | ⚠️ Parcial | Keys em volume efêmero |
| Containerização | ✅ OK | Dockerfile presente |
| Configuração | ⚠️ Parcial | Rigidez nas env vars |

**Subnota:** 5.0/10

---

### 👁️ Observabilidade (2.0/10)

| Critério | Status | Observação |
|----------|--------|------------|
| Logging estruturado | ❌ Ausente | Sem Serilog |
| Correlation ID | ❌ Ausente | Impossível rastrear |
| Métricas | ❌ Ausente | Sem Prometheus |
| Tracing distribuído | ❌ Ausente | Sem OpenTelemetry |

**Subnota:** 2.0/10

---

### ⚡ Performance (6.0/10)

| Critério | Status | Observação |
|----------|--------|------------|
| YARP nativo | ✅ OK | Proxy moderno |
| Cache de rotas | ✅ OK | Configuração em memória |
| Compressão | ⚠️ Não verificado | Falta configurar |
| Timeouts | ⚠️ Não verificado | Depende dos clusters |

**Subnota:** 6.0/10

---

### ⚙️ Configurabilidade (5.0/10)

| Critério | Status | Observação |
|----------|--------|------------|
| Configuration-driven | ✅ OK | appsettings.json |
| Environment variables | ⚠️ Parcial | Algumas chaves |
| Secrets management | ❌ Ausente | Sem Key Vault |
| Feature flags | ❌ Ausente | Não suportado |

**Subnota:** 5.0/10

---

## 4.3 Comparativo de Evolução

| Data | Score | Delta | Motivo |
|------|-------|-------|--------|
| 2026-03-03 | 6.5/10 | - | Code Review inicial |
| 2026-03-23 | 3.9/10 | -2.6 | Reavaliação com critérios mais rigorosos |

> **Nota:** A diferença reflete uma análise mais detalhada das implicações de segurança em produção.

---

## 4.4 Classificação

| Faixa | Classificação | Ação Recomendada |
|-------|---------------|------------------|
| 0.0 - 2.9 | 🛑 Crítico | Não utilizar em produção |
| 3.0 - 4.9 | 🔴 Insuficiente | Corrigir issues críticos primeiro |
| 5.0 - 6.9 | 🟡 Atenção | Melhorias incrementais necessárias |
| 7.0 - 8.9 | 🟢 Bom | Estável com monitoramento |
| 9.0 - 10.0 | 🟢⭐ Excelente | Pronto para produção |

**Classificação Atual:** 🔴 **INSUFICIENTE (3.9/10)**

---

# 5. PLANO DE AÇÃO

## Prioridade 1 - Correções Críticas

| # | Issue ID | Descrição | Esforço | Status |
|---|----------|-----------|---------|--------|
| 1 | APIGW-001 | Habilitar AuthorizationPolicy em todas as rotas | 15 min | ⬜ |
| 2 | APIGW-002 | Remover JWT Secret hardcoded | 30 min | ⬜ |

## Prioridade 2 - Alta Prioridade

| # | Issue ID | Descrição | Esforço | Status |
|---|----------|-----------|---------|--------|
| 3 | APIGW-003 | Implementar rate limiting | 1h | ⬜ |
| 4 | APIGW-004 | Adicionar health checks | 30 min | ⬜ |
| 5 | APIGW-005 | Configurar logging estruturado | 2h | ⬜ |
| 6 | APIGW-006 | Adicionar correlation ID middleware | 1h | ⬜ |

## Prioridade 3 - Média Prioridade

| # | Issue ID | Descrição | Esforço | Status |
|---|----------|-----------|---------|--------|
| 7 | APIGW-007 | CORS configurável por ambiente | 30 min | ⬜ |
| 8 | APIGW-008 | Melhorar persistência de Data Protection Keys | 1h | ⬜ |
| 9 | APIGW-009 | Rate limiting por IP/cliente | 1h | ⬜ |
| 10 | APIGW-010 | URLs configuráveis via environment variables | 1h | ⬜ |

---

## Tempo Total Estimado

| Prioridade | Tempo |
|------------|-------|
| Crítica | 45 min |
| Alta | 4.5 h |
| Média | 3.5 h |
| **Total** | **~8.5 horas** |

---

# 6. REFERÊNCIAS

- [YARP Documentation](https://microsoft.github.io/reverse-proxy/)
- [Security Best Practices for YARP](https://microsoft.github.io/reverse-proxy/docs/security/)
- [Rate Limiting in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limiting)
- [Health Checks in YARP](https://microsoft.github.io/reverse-proxy/docs/api-health/)
- [Distributed Tracing with OpenTelemetry](https://opentelemetry.io/)

---

**Documento criado em:** 2026-03-23  
**Última atualização:** 2026-03-23  
**Versão:** 1.0
