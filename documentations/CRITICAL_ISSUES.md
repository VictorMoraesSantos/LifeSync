# LifeSync - Problemas Críticos e Recomendações

> **Data:** 23 de março de 2026  
> **Última Atualização:** 23 de março de 2026  
> **Total de Issues:** 120+

---

## Índice

1. [Resumo Executivo](#resumo-executivo)
2. [Users Service](#users-service)
3. [TaskManager Service](#taskmanager-service)
4. [Nutrition Service](#nutrition-service)
5. [Financial Service](#financial-service)
6. [Gym Service](#gym-service)
7. [Notification Service](#notification-service)
8. [BuildingBlocks](#buildingblocks)
9. [API Gateway](#api-gateway)
10. [ClientApp (MAUI)](#clientapp-maui)
11. [Plano de Ação Consolidado](#plano-de-ação-consolidado)

---

## Resumo Executivo

### Scores por Serviço

| Serviço | Score | Status |
|---------|-------|--------|
| TaskManager Service | 6.5/10 | ⚠️ Atención |
| API Gateway | 6.5/10 | ⚠️ Atención |
| Nutrition Service | 5.5/10 | 🔴 Crítico |
| Financial Service | 5.5/10 | 🔴 Crítico |
| Users Service | 5.0/10 | 🔴 Crítico |
| Gym Service | 5.0/10 | 🔴 Crítico |
| BuildingBlocks | 5.0/10 | 🔴 Crítico |
| ClientApp | 5.5/10 | 🔴 Crítico |
| Notification Service | 3.5/10 | 🔴 Crítico |

### Issues por Severidade (Total)

| Severidade | Quantidade |
|------------|------------|
| 🔴 CRÍTICO | 45+ |
| 🟠 ALTO | 50+ |
| 🟡 MÉDIO | 25+ |
| 🔵 BAIXO | 5+ |

### Problemas Comuns a Todos os Serviços

| # | Problema | Impacto | Severidade |
|---|----------|---------|------------|
| 1 | JWT Secret hardcoded | Segurança comprometida | 🔴 CRÍTICO |
| 2 | `[Authorize]` ausente/comentado | Endpoints públicos | 🔴 CRÍTICO |
| 3 | Paginação em memória | Performance O(n) | 🟠 ALTO |
| 4 | Sem Global Query Filter | Soft delete não filtrado | 🟠 ALTO |
| 5 | Sem rate limiting | Vulnerável a DoS | 🟠 ALTO |
| 6 | Sem logging/observabilidade | Difícil troubleshooting | 🟡 MÉDIO |

---

## Users Service

> **Documentação:** `services/users-service.md`  
> **Score:** 5/10

### Problemas Críticos

| # | Severidade | Problema | Impacto | Arquivo |
|---|------------|----------|---------|---------|
| US-001 | 🔴 CRÍTICO | Política de senha fraca (6 chars, sem special chars) | Vulnerável a bruteforce | `IdentityOptions` |
| US-002 | 🔴 CRÍTICO | JWT Secret hardcoded | Chave exposta em código | `appsettings.json` |
| US-003 | 🔴 CRÍTICO | Sem token blacklist/revogação | Access token válido após logout | `AuthService.cs` |
| US-004 | 🔴 CRÍTICO | Soft delete não filtrado | Usuários deletados fazem login | `UsersController.cs` |
| US-005 | 🔴 CRÍTICO | Bug no UpdateUser (command ignorado) | Update não funciona | `UsersController.cs:PutUser` |
| US-006 | 🔴 CRÍTICO | N+1 em GetAllUsersDetailsAsync | 1 query por usuário | `UserService.cs` |
| US-007 | 🔴 CRÍTICO | DeleteUserCommandHandler vazio | Delete não funciona | `DeleteUserCommandHandler.cs` |
| US-008 | 🟠 ALTO | LastLoginAt nunca atualizado | Campo inútil | `AuthService.cs` |
| US-009 | 🟠 ALTO | E-mail alterado sem verificação | Segurança reduzida | `UsersController.cs` |
| US-010 | 🟠 ALTO | CORS permissivo | AllowedAnyMethod/Header | `Program.cs` |
| US-011 | 🟠 ALTO | Refresh token expiry hardcoded | Ignora configuração | `AuthService.cs` |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Adicionar política de senha OWASP (12+ chars, special) | 1h |
| P1 | 2 | Remover JWT do appsettings, usar Secret Manager | 30min |
| P1 | 3 | Implementar token blacklist com Redis | 4h |
| P1 | 4 | Adicionar Global Query Filter para IsDeleted | 1h |
| P1 | 5 | Corrigir bug no UpdateUser command | 15min |
| P1 | 6 | Corrigir N+1 com Include para roles | 1h |
| P1 | 7 | Implementar DeleteUserCommandHandler | 2h |
| **P2 - Altos** | 8 | Atualizar LastLoginAt no login | 15min |
| P2 | 9 | Requerer confirmação de e-mail na mudança | 2h |
| P2 | 10 | Restringir CORS para origens específicas | 30min |

---

## TaskManager Service

> **Documentação:** `services/taskmanager-service.md`  
> **Score:** 6.5/10

### Problemas Críticos

| # | Severidade | Problema | Impacto | Arquivo |
|---|------------|----------|---------|---------|
| TM-001 | 🔴 CRÍTICO | `[Authorize]` comentado no ApiController | API sem autenticação | `ApiController.cs` |
| TM-002 | 🔴 CRÍTICO | JWT Key hardcoded no appsettings | Segurança comprometida | `appsettings.json` |
| TM-003 | 🔴 CRÍTICO | UserId injetado via body (não do JWT) | Qualquer usuário pode falsificar | `CreateTaskItemCommand` |
| TM-004 | 🟠 ALTO | Paginação em memória | Carrega todos antes de paginar | `GetTaskItemsByFilterQueryHandler` |
| TM-005 | 🟠 ALTO | SaveChanges individual em batch | Performance ruim | `CreateTaskItemBatchHandler` |
| TM-006 | 🟠 ALTO | Sem Global Query Filter | Soft delete não filtrado | `ApplicationDbContext` |
| TM-007 | 🟠 ALTO | DueDateReminderService sem controle de duplicatas | Lembretes duplicados | `DueDateReminderService` |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Descomentar `[Authorize]` no ApiController | 5min |
| P1 | 2 | Remover JWT hardcoded, usar env var | 30min |
| P1 | 3 | Extrair UserId do JWT claims | 1h |
| **P2 - Altos** | 4 | Implementar paginação no banco (Skip/Take) | 2h |
| P2 | 5 | Usar Unit of Work para batch operations | 2h |
| P2 | 6 | Adicionar Global Query Filter para IsDeleted | 1h |
| P2 | 7 | Adicionar controle de duplicatas no reminder | 1h |

---

## Nutrition Service

> **Documentação:** `services/nutrition-service.md`  
> **Score:** 5.5/10

### Problemas Críticos

| # | Severidade | Problema | Impacto | Arquivo |
|---|------------|----------|---------|---------|
| NT-001 | 🔴 CRÍTICO | Entidade Food anêmica (public setters) | Zero encapsulamento | `Food.cs` |
| NT-002 | 🔴 CRÍTICO | Bug NullReference em Meal.RemoveMealFood() | Lança exceção | `Meal.cs` |
| NT-003 | 🔴 CRÍTICO | Paginação em memória (3 services) | Performance O(n) | `*Service.cs` |
| NT-004 | 🔴 CRÍTICO | FindAsync carrega tudo em memória | Filtragem após conversão | `*Repository.cs` |
| NT-005 | 🔴 CRÍTICO | Sem `[Authorize]` nos controllers | Endpoints públicos | `*Controller.cs` |
| NT-006 | 🔴 CRÍTICO | JWT Secret hardcoded | Segurança comprometida | `appsettings.json` |
| NT-007 | 🟠 ALTO | Ausência de FluentValidation | Sem validação de input | `Commands` |
| NT-008 | 🟠 ALTO | Event handlers com falha silenciosa | Erros não tratados | `MealFoodAddedEventHandler.cs` |
| NT-009 | 🟠 ALTO | Sem índices no banco | Queries lentas | `ApplicationDbContext` |
| NT-010 | 🟠 ALTO | Sem isolamento de dados por usuário | Vazamento de dados | Repository |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Encapsular Food (private setters + validação) | 2h |
| P1 | 2 | Corrigir NullReference em RemoveMealFood() | 15min |
| P1 | 3 | Corrigir paginação em memória (3 services) | 3h |
| P1 | 4 | Corrigir FindAsync para filtrar no banco | 2h |
| P1 | 5 | Adicionar `[Authorize]` nos controllers | 10min |
| P1 | 6 | Remover JWT hardcoded | 30min |
| **P2 - Altos** | 7 | Adicionar FluentValidation nos commands | 4h |
| P2 | 8 | Corrigir event handlers com try-catch | 1h |
| P2 | 9 | Adicionar índices (UserId, Date, DiaryId, MealId) | 30min |

---

## Financial Service

> **Documentação:** `services/financial-service.md`  
> **Score:** 5.5/10

### Problemas Críticos

| # | Severidade | Problema | Impacto | Arquivo |
|---|------------|----------|---------|---------|
| FS-001 | 🔴 CRÍTICO | Bug dupla atribuição em Transaction.Update() | Setter perde propósito | `Transaction.cs` |
| FS-002 | 🔴 CRÍTICO | Money não permite valores negativos | Impossível representar despesas | `Money.cs` |
| FS-003 | 🔴 CRÍTICO | Paginação em memória | Carrega 100k registros | `CategoryService.cs` |
| FS-004 | 🔴 CRÍTICO | N+1 em DeleteRangeAsync | N queries para deletar N registros | `CategoryService.cs` |
| FS-005 | 🔴 CRÍTICO | Sem `[Authorize]` nos controllers | Todos públicos | `CategoriesController.cs` |
| FS-006 | 🔴 CRÍTICO | JWT Secret hardcoded | Segurança comprometida | `appsettings.json` |
| FS-007 | 🔴 CRÍTICO | GetByNameAsync ignora userId | Vazamento de dados | `CategoryService.cs` |
| FS-008 | 🟠 ALTO | Validador sempre true (.Must) | Validação ineficaz | `CreateTransactionCommandValidator` |
| FS-009 | 🟠 ALTO | Índices faltando | Queries lentas | `ApplicationDbContext` |
| FS-010 | 🟠 ALTO | Sem Global Query Filter | IsDeleted não filtrado | `ApplicationDbContext` |
| FS-011 | 🟠 ALTO | Inconsistência de exceções | DomainException vs ArgumentOutOfRange | `Category.cs`, `Transaction.cs` |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Adicionar `[Authorize]` em todos os controllers | 10min |
| P1 | 2 | Remover JWT hardcoded | 30min |
| P1 | 3 | Corrigir paginação em memória (IQueryable) | 2h |
| P1 | 4 | Corrigir N+1 no DeleteRangeAsync | 1h |
| P1 | 5 | Corrigir bug dupla atribuição + typo SetAmout | 15min |
| P1 | 6 | Corrigir validador sempre true | 10min |
| P1 | 7 | Filtrar GetByNameAsync por userId | 30min |
| **P2 - Altos** | 8 | Adicionar índices no banco | 30min |
| P2 | 9 | Adicionar Global Query Filter | 30min |
| P2 | 10 | Padronizar exceções (DomainException) | 1h |

---

## Gym Service

> **Documentação:** `services/gym-service.md`  
> **Score:** 5.0/10

### Problemas Críticos

| # | Severidade | Problema | Impacto | Arquivo |
|---|------------|----------|---------|---------|
| GY-001 | 🔴 CRÍTICO | RoutineMapper com parâmetros invertidos | Name/Description trocados | `RoutineMapper.cs` |
| GY-002 | 🔴 CRÍTICO | Paginação em memória | Performance O(n) | `*Service.cs` |
| GY-003 | 🔴 CRÍTICO | Sem `[Authorize]` nos controllers | Endpoints públicos | `*Controller.cs` |
| GY-004 | 🔴 CRÍTICO | JWT Secret hardcoded | Segurança comprometida | `appsettings.json` |
| GY-005 | 🔴 CRÍTICO | RoutineExerciseSpecification bug (<= vs ==) | Especificação incorreta | `RoutineExerciseSpecification` |
| GY-006 | 🟠 ALTO | CompletedExerciseConfiguration duplicada | Configuração conflitante | `ApplicationDbContext` |
| GY-007 | 🟠 ALTO | CompletedAt nunca inicializado | Valor default 01/01/0001 | `CompletedExercise.cs` |
| GY-008 | 🟠 ALTO | Sem isolamento de dados por usuário | Dados acessíveis entre usuários | Repository |
| GY-009 | 🟠 ALTO | TrainingSessionSpecification NullReference | Pode lançar exceção | `TrainingSessionSpecification` |
| GY-010 | 🟠 ALTO | Arquivos com extensão duplicada (.cs.cs) | Bug de build | `*.cs.cs` |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Corrigir RoutineMapper (inverter parâmetros) | 15min |
| P1 | 2 | Implementar paginação no banco | 2h |
| P1 | 3 | Adicionar `[Authorize]` nos controllers | 10min |
| P1 | 4 | Remover JWT hardcoded | 30min |
| P1 | 5 | Corrigir SetsEquals (== ao invés de <=) | 10min |
| **P2 - Altos** | 6 | Remover CompletedExerciseConfiguration duplicada | 15min |
| P2 | 7 | Inicializar CompletedAt no construtor | 10min |
| P2 | 8 | Implementar isolamento por UserId | 2h |
| P2 | 9 | Adicionar null-check em Specification | 15min |
| P2 | 10 | Renomear arquivos .cs.cs | 5min |

---

## Notification Service

> **Documentação:** `services/notification-service.md`  
> **Score:** 3.5/10 ⚠️ **MAIOR RISCO**

### Problemas Críticos

| # | Severidade | Problema | Impacto | Arquivo |
|---|------------|----------|---------|---------|
| NF-001 | 🔴 CRÍTICO | Consumer sem try-catch | JSON malformado causa crash | `RabbitMqEventConsumer.cs` |
| NF-002 | 🔴 CRÍTICO | Blocking async (.GetAwaiter().GetResult()) | Thread starvation | `EventBus.cs` |
| NF-003 | 🔴 CRÍTICO | SMTP sem TLS (SecureSocketOptions.None) | Credenciais em texto puro | `EmailService.cs` |
| NF-004 | 🔴 CRÍTICO | Sem error handling no EmailService | Exceções não tratadas | `EmailService.cs` |
| NF-005 | 🔴 CRÍTICO | Credenciais RabbitMQ hardcoded (guest/guest) | Segurança comprometida | `appsettings.json` |
| NF-006 | 🟠 ALTO | Sem Dead Letter Queue | Mensagens perdidas | RabbitMQ config |
| NF-007 | 🟠 ALTO | Zero logging em todo o serviço | Impossível troubleshoot | Todos |
| NF-008 | 🟠 ALTO | Zero testes | Sem cobertura | `NotificationService.Tests` |
| NF-009 | 🟠 ALTO | Banco implementado mas nunca usado | Dead code | `EmailMessageRepository` |
| NF-010 | 🟠 ALTO | Handler para TaskDueReminder ausente | Evento ignorado | `*Handler.cs` |
| NF-011 | 🟠 ALTO | Nova conexão SMTP por e-mail | Overhead de conexão | `EmailService.cs` |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Adicionar try-catch no consumer | 15min |
| P1 | 2 | Converter GetAwaiter().GetResult() para await | 1h |
| P1 | 3 | Habilitar TLS no SmtpClient | 15min |
| P1 | 4 | Adicionar error handling no EmailService | 30min |
| P1 | 5 | Remover credenciais hardcoded | 30min |
| **P2 - Altos** | 6 | Implementar Dead Letter Queue | 2h |
| P2 | 7 | Adicionar ILogger em todos os serviços | 2h |
| P2 | 8 | Implementar testes unitários | 8h |
| P2 | 9 | Remover banco não utilizado OU usar | 1h |
| P2 | 10 | Implementar TaskDueReminderHandler | 2h |
| P2 | 11 | Implementar SMTP connection pooling | 2h |

---

## BuildingBlocks

> **Documentação:** `services/building-blocks.md`  
> **Score:** 5.0/10

### Problemas Críticos

| # | Severidade | Problema | Impacto | Biblioteca |
|---|------------|----------|---------|------------|
| BB-001 | 🔴 CRÍTICO | Result<T>.Value lança exceção em falha | Comportamento inesperado | BuildingBlocks.Results |
| BB-002 | 🔴 CRÍTICO | ValidationBehavior interrompe com exceção | Pipeline quebrado | BuildingBlocks.CQRS |
| BB-003 | 🔴 CRÍTICO | JWT Key exposta em exception message | Informação sensível exposta | BuildingBlocks.Auth |
| BB-004 | 🟠 ALTO | Domain Events sem mecanismo de dispatch | Eventos não publicados | Core.Domain |
| BB-005 | 🟠 ALTO | Repository sem Unit of Work | Transações não gerenciadas | Core.Infrastructure |
| BB-006 | 🟠 ALTO | RabbitMQ sem validação de config | Falhas silenciosas | BuildingBlocks.Messaging |
| BB-007 | 🟡 MÉDIO | Task.WhenAll sem ConfigureAwait(false) | Possíveis deadlocks | BuildingBlocks.CQRS |
| BB-008 | 🟡 MÉDIO | ExchangeDeclare com GetAwaiter().GetResult() | Blocking async | BuildingBlocks.Messaging |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Corrigir Result<T>.Value para não lançar | 2h |
| P1 | 2 | ValidationBehavior deve retornar Result, não lançar | 1h |
| P1 | 3 | Remover exposição de JWT Key em exceptions | 30min |
| **P2 - Altos** | 4 | Implementar Domain Event Dispatcher | 4h |
| P2 | 5 | Adicionar Unit of Work pattern | 6h |
| P2 | 6 | Adicionar validação de config no startup | 1h |
| **P3 - Médios** | 7 | Adicionar ConfigureAwait(false) | 15min |
| P3 | 8 | Converter blocking calls para async | 2h |

---

## API Gateway

> **Documentação:** `architecture/API-GATEWAY.md`  
> **Score:** 6.5/10

### Problemas Críticos

| # | Severidade | Problema | Impacto | Arquivo |
|---|------------|----------|---------|---------|
| APIGW-001 | 🔴 CRÍTICO | AuthorizationPolicy comentada nas rotas | Apenas gym usa auth | `appsettings.json` |
| APIGW-002 | 🔴 CRÍTICO | JWT Secret hardcoded | Chave exposta | `appsettings.json` |
| APIGW-003 | 🟠 ALTO | Sem Rate Limiting | Vulnerável a DoS | `Program.cs` |
| APIGW-004 | 🟠 ALTO | Sem Health Checks | Orquestradores cegos | `Program.cs` |
| APIGW-005 | 🟠 ALTO | Sem Logging/Request Tracing | Sem correlation ID | `Program.cs` |
| APIGW-006 | 🟠 ALTO | Sem Correlation ID Middleware | Difícil troubleshooting | `Program.cs` |
| APIGW-007 | 🟡 MÉDIO | CORS apenas localhost | Sem suporte produção | `appsettings.json` |
| APIGW-008 | 🟡 MÉDIO | Data Protection Keys em volume Docker | Risco de perda | `Program.cs` |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Habilitar AuthorizationPolicy em todas as rotas | 15min |
| P1 | 2 | Remover JWT hardcoded, usar env var | 30min |
| **P2 - Altos** | 3 | Implementar Rate Limiting | 1h |
| P2 | 4 | Adicionar Health Checks | 30min |
| P2 | 5 | Configurar Serilog + Correlation ID | 1h |
| **P3 - Médios** | 6 | Configurar CORS para produção | 30min |
| P3 | 7 | Persistir Data Protection Keys external storage | 1h |

---

## ClientApp (MAUI)

> **Documentação:** `architecture/CLIENTAPP.md`  
> **Score:** 5.5/10

### Problemas Críticos

| # | Severidade | Problema | Impacto | Arquivo |
|---|------------|----------|---------|---------|
| CL-001 | 🔴 CRÍTICO | URL da API hardcoded | Sem configuração por ambiente | `MauiProgram.cs` |
| CL-002 | 🔴 CRÍTICO | Sem refresh token automático | Usuário deslogado ao expirar | `AuthDelegatingHandler.cs` |
| CL-003 | 🔴 CRÍTICO | UserId pode ser 0 | Chamadas com userId inválido | `UserSession.cs` |
| CL-004 | 🟠 ALTO | Sem Polly/Resiliência HTTP | Sem retry/circuit breaker | `Services/*` |
| CL-005 | 🟠 ALTO | ViewModels como Singleton | Estado inconsistente | `MauiProgram.cs` |
| CL-006 | 🟠 ALTO | Módulo Gym não implementado | Funcionalidade incompleta | `AcademicPage` |
| CL-007 | 🟠 ALTO | Cobertura de testes mínima | Risco de regressões | `LifeSyncApp.Tests` |
| CL-008 | 🟡 MÉDIO | IsBusy não dispara OnPropertyChanged | UI não atualiza | `BaseViewModel.cs` |
| CL-009 | 🟡 MÉDIO | Sem timeout em HttpClient | Requests podem hang | `ApiService.cs` |
| CL-010 | 🟡 MÉDIO | SSL bypass no Android | Segurança reduzida | `MauiProgram.cs` |

### Recomendações de Correção

| Prioridade | # | Ação | Esforço |
|------------|---|------|---------|
| **P1 - Críticos** | 1 | Externalizar API URL (appsettings.json) | 1h |
| P1 | 2 | Implementar refresh token automático | 3h |
| P1 | 3 | Tratar UserId=0 como erro | 30min |
| **P2 - Altos** | 4 | Adicionar Polly (retry, circuit breaker) | 2h |
| P2 | 5 | Revisar ViewModels Singleton vs Transient | 1h |
| P2 | 6 | Implementar módulo Gym | 8h |
| P2 | 7 | Expandir cobertura de testes | 16h |
| **P3 - Médios** | 8 | Corrigir IsBusy OnPropertyChanged | 15min |
| P3 | 9 | Adicionar HttpClient timeout | 10min |
| P3 | 10 | Remover SSL bypass em produção | 5min |

---

## Plano de Ação Consolidado

### Semana 1 — Críticos de Segurança (TODOS serviços)

| # | Ação | Responsável | Impacto |
|---|------|-------------|---------|
| 1 | Remover JWT Secret hardcoded de todos os serviços | Dev | 🔴 CRÍTICO |
| 2 | Adicionar `[Authorize]` em todos os controllers | Dev | 🔴 CRÍTICO |
| 3 | Habilitar AuthorizationPolicy no API Gateway | Dev | 🔴 CRÍTICO |
| 4 | Implementar token blacklist no Users Service | Dev | 🔴 CRÍTICO |
| 5 | Corrigir política de senha (OWASP) | Dev | 🔴 CRÍTICO |

### Semana 2 — Performance Crítica

| # | Ação | Responsável | Impacto |
|---|------|-------------|---------|
| 6 | Corrigir paginação em memória (TODOS) | Dev | 🟠 ALTO |
| 7 | Corrigir N+1 queries | Dev | 🟠 ALTO |
| 8 | Adicionar índices no banco | Dev | 🟠 ALTO |
| 9 | Implementar Global Query Filter | Dev | 🟠 ALTO |

### Semana 3 — Error Handling e Observabilidade

| # | Ação | Responsável | Impacto |
|---|------|-------------|---------|
| 10 | Adicionar try-catch no Notification Consumer | Dev | 🔴 CRÍTICO |
| 11 | Implementar logging em todos os serviços | Dev | 🟠 ALTO |
| 12 | Adicionar Health Checks no API Gateway | Dev | 🟠 ALTO |
| 13 | Implementar Rate Limiting | Dev | 🟠 ALTO |

### Semana 4 — Correções de Bugs

| # | Ação | Responsável | Impacto |
|---|------|-------------|---------|
| 14 | Corrigir RoutineMapper (Name/Description) | Dev | 🔴 CRÍTICO |
| 15 | Corrigir Transaction.Update() typo | Dev | 🔴 CRÍTICO |
| 16 | Corrigir Meal.RemoveMealFood() NullRef | Dev | 🔴 CRÍTICO |
| 17 | Corrigir Money para permitir negativos | Dev | 🔴 CRÍTICO |
| 18 | Implementar DeleteUserCommandHandler | Dev | 🟠 ALTO |

### Semana 5-6 — Melhorias de Arquitetura

| # | Ação | Responsável | Impacto |
|---|------|-------------|---------|
| 19 | Implementar Unit of Work | Dev | 🟡 MÉDIO |
| 20 | Implementar Domain Event Dispatcher | Dev | 🟡 MÉDIO |
| 21 | Adicionar FluentValidation | Dev | 🟡 MÉDIO |
| 22 | Implementar Refresh Token no ClientApp | Dev | 🔴 CRÍTICO |

### Semana 7-8 — Testes e Quality Assurance

| # | Ação | Responsável | Impacto |
|---|------|-------------|---------|
| 23 | Implementar testes unitários (~335/serviço) | Dev | 🟡 MÉDIO |
| 24 | Implementar testes de integração | Dev | 🟡 MÉDIO |
| 25 | Implementar testes E2E | QA | 🟡 MÉDIO |
| 26 | Code review e refatoração | Dev | 🟡 MÉDIO |

---

## Estimativa de Esforço Total

| Categoria | Issues | Horas Estimadas |
|-----------|--------|-----------------|
| Críticos (P1) | 45+ | ~60h |
| Altos (P2) | 50+ | ~120h |
| Médios (P3) | 25+ | ~80h |
| **TOTAL** | **120+** | **~260h** |

---

## Priorização Final

### MUST FIX (Antes de Produção)

1. JWT Secret hardcoded (TODOS)
2. `[Authorize]` ausente (TODOS)
3. Paginação em memória (Performance)
4. Notification Consumer sem try-catch (Crash)
5. SMTP sem TLS (Segurança)
6. RoutineMapper invertido (Bug em produção)

### SHOULD FIX (Sprint seguinte)

1. Rate Limiting
2. Health Checks
3. Global Query Filter
4. Unit of Work
5. Refresh Token no ClientApp

### NICE TO HAVE (Backlog)

1. Polly/Resiliência HTTP
2. Correlation ID / Logging estruturado
3. Testes de integração
4. Domain Event Dispatcher

---

> **Última atualização:** 23 de março de 2026  
> **Próxima revisão:** 30 de março de 2026
