# 📚 Documentação do LifeSync

Documentação técnica completa do ecossistema de microserviços LifeSync — arquitetura, serviços, code reviews, planos de teste e guias de deploy.

**Navegação Rápida:** [🧩 Serviços](#-documentação-dos-microserviços) · [🏗️ Arquitetura](#️-arquitetura) · [🔍 Code Reviews](#-code-reviews) · [🧪 Testes](#-planos-de-teste) · [🚀 Deploy](#-deployment) · [⚠️ Issues](#️-análises-estratégicas)

---

## 📁 Estrutura da Documentação

```
documentations/
├── services/
│   ├── openapi/           → OpenAPI 3.0 specs (YAML) por serviço
│   ├── diagrams/          → Diagramas C4 e sequência (Mermaid)
│   └── *.md              → Documentação técnica detalhada de cada microserviço
├── architecture/
│   ├── adr/              → Architecture Decision Records (ADR-001, ADR-002, etc.)
│   └── *.md              → Decisões arquiteturais, roadmap e tarefas
├── code-reviews/           → Code reviews consolidados por serviço
├── test-plans/             → Planos de teste por microserviço
├── deployment/             → Guias de deploy e configuração
├── ANALISE-ESTRATEGICA-COMPLETA.md
└── CRITICAL_ISSUES.md
```

---

## 🧩 Documentação dos Microserviços

Cada arquivo cobre: visão geral, estrutura de pastas, modelo de domínio (entidades, value objects, enums, eventos, erros), camada de aplicação (commands, queries, handlers, validators, DTOs, contratos), infraestrutura (DbContext, repositórios, migrations), **API Examples (exemplos curl)**, **Erros (catálogo)**, endpoints da API, configuração e dependências.

| Serviço | Arquivo | OpenAPI Spec |
|---------|---------|--------------|
| 👤 **Users** | [users-service.md](./services/users-service.md) | [openapi.yaml](./services/openapi/users-openapi.yaml) |
| ✅ **TaskManager** | [taskmanager-service.md](./services/taskmanager-service.md) | [openapi.yaml](./services/openapi/taskmanager-openapi.yaml) |
| 🥗 **Nutrition** | [nutrition-service.md](./services/nutrition-service.md) | [openapi.yaml](./services/openapi/nutrition-openapi.yaml) |
| 💰 **Financial** | [financial-service.md](./services/financial-service.md) | [openapi.yaml](./services/openapi/financial-openapi.yaml) |
| 🏋️ **Gym** | [gym-service.md](./services/gym-service.md) | [openapi.yaml](./services/openapi/gym-openapi.yaml) |
| 📧 **Notification** | [notification-service.md](./services/notification-service.md) | [openapi.yaml](./services/openapi/notification-openapi.yaml) |

### Diagramas Arquiteturais

| Diagrama | Arquivo | Descrição |
|----------|---------|-----------|
| Users Service Context | [users-service-context.mmd](./services/diagrams/users-service-context.mmd) | C4 Context - Users API, PostgreSQL, RabbitMQ |
| Financial Service Context | [financial-service-context.mmd](./services/diagrams/financial-service-context.mmd) | C4 Context - Financial API, PostgreSQL |
| User Registration Flow | [user-registration-sequence.mmd](./services/diagrams/user-registration-sequence.mmd) | Sequência - registro → RabbitMQ → Notification |
| Recurring Transaction Flow | [recurring-transaction-sequence.mmd](./services/diagrams/recurring-transaction-sequence.mmd) | Sequência - BackgroundService → Transaction generation |

### Documentação Complementar de Serviços

| Arquivo | Descrição |
|---------|-----------|
| [RecurringTransactions.md](./services/RecurringTransactions.md) | Decisão arquitetural: fluxo de transações recorrentes |
| [NUTRITION_DOCUMENTATION.md](./services/NUTRITION_DOCUMENTATION.md) | Documentação estendida do Nutrition Service |
| [TASKMANAGER_DOCUMENTATION.md](./services/TASKMANAGER_DOCUMENTATION.md) | Documentação estendida do TaskManager Service |
| [MAUI-TESTS-README.md](./services/MAUI-TESTS-README.md) | Guia de testes MAUI |

---

## 🏗️ Arquitetura

| Arquivo | Descrição |
|---------|-----------|
| [API-GATEWAY.md](./architecture/API-GATEWAY.md) | Configuração e roteamento do YARP API Gateway |
| [CLIENTAPP.md](./architecture/CLIENTAPP.md) | Arquitetura do frontend Blazor WebAssembly |
| [DOCUMENTATION.md](./architecture/DOCUMENTATION.md) | Padrões de documentação do projeto |
| [FEATURE-ROADMAP.md](./architecture/FEATURE-ROADMAP.md) | Roadmap de features futuras |
| [IMPLEMENTATION-SUMMARY.md](./architecture/IMPLEMENTATION-SUMMARY.md) | Resumo de implementação |
| [IMPLEMENTATION-TASKS.md](./architecture/IMPLEMENTATION-TASKS.md) | Tarefas de implementação pendentes |

### Architecture Decision Records (ADRs)

| ADR | Título | Descrição |
|-----|--------|-----------|
| [ADR-001](./architecture/adr/ADR-001-email-strategy-pattern.md) | Strategy Pattern for Email Templates | Strategy Pattern no Notification Service para resolver templates de email |
| [ADR-002](./architecture/adr/ADR-002-money-value-object.md) | Money Value Object | Value Object Money para suporte multi-moeda no Financial Service |
| [ADR-003](./architecture/adr/ADR-003-rabbitmq-integration.md) | RabbitMQ for Service Integration | Uso de RabbitMQ como event bus para integração entre serviços |
| [ADR-004](./architecture/adr/ADR-004-cqrs-result-pattern.md) | CQRS + Result Pattern | Adoção de CQRS + Result Pattern em todos os serviços |

---

## 🔍 Code Reviews

Análises de qualidade de código realizadas por serviço com classificação por severidade (Crítico, Alto, Médio, Baixo) e recomendações de correção.

| Arquivo | Serviço | Score |
|---------|---------|-------|
| [LIFESYNC_CODE_REVIEW_CONSOLIDADO.md](./code-reviews/LIFESYNC_CODE_REVIEW_CONSOLIDADO.md) | **Consolidado (todos os serviços)** | — |
| [USERS_CODE_REVIEW.md](./code-reviews/USERS_CODE_REVIEW.md) | Users Service | — |
| [TASKMANAGER_CODE_REVIEW.md](./code-reviews/TASKMANAGER_CODE_REVIEW.md) | TaskManager Service | — |
| [NUTRITION_CODE_REVIEW.md](./code-reviews/NUTRITION_CODE_REVIEW.md) | Nutrition Service | 5.5/10 |
| [FINANCIAL_CODE_REVIEW.md](./code-reviews/FINANCIAL_CODE_REVIEW.md) | Financial Service | — |
| [GYM_CODE_REVIEW.md](./code-reviews/GYM_CODE_REVIEW.md) | Gym Service | 5/10 |
| [NOTIFICATION_CODE_REVIEW.md](./code-reviews/NOTIFICATION_CODE_REVIEW.md) | Notification Service | 3.5/10 |
| [APIGATEWAY_WEBAPP_CODE_REVIEW.md](./code-reviews/APIGATEWAY_WEBAPP_CODE_REVIEW.md) | API Gateway + WebApp | — |

---

## 🧪 Planos de Teste

Planos detalhados de teste com cenários, critérios de aceitação e cobertura esperada.

| Arquivo | Serviço |
|---------|---------|
| [Financial-Test-Plan.md](./test-plans/Financial-Test-Plan.md) | Financial Service |
| [Gym-Test-Plan.md](./test-plans/Gym-Test-Plan.md) | Gym Service |
| [Nutrition-Test-Plan.md](./test-plans/Nutrition-Test-Plan.md) | Nutrition Service |
| [TaskManager-Test-Plan.md](./test-plans/TaskManager-Test-Plan.md) | TaskManager Service |

---

## 🚀 Deployment

| Arquivo | Descrição |
|---------|-----------|
| [DEPLOY-VPS-DOCKER.md](./deployment/DEPLOY-VPS-DOCKER.md) | Guia de deploy em VPS com Docker |
| [GOOGLE-AUTH-IMPLEMENTATION.md](./deployment/GOOGLE-AUTH-IMPLEMENTATION.md) | Implementação da autenticação Google OAuth |

---

## ⚠️ Análises Estratégicas

| Arquivo | Descrição |
|---------|-----------|
| [ANALISE-ESTRATEGICA-COMPLETA.md](./ANALISE-ESTRATEGICA-COMPLETA.md) | Análise estratégica completa do projeto |
| [CRITICAL_ISSUES.md](./CRITICAL_ISSUES.md) | Issues críticas consolidadas que requerem atenção imediata |

---

[← Voltar ao README do Projeto](../README.md)
