# Documentação dos Microserviços — LifeSync

Esta pasta contém a documentação técnica detalhada de cada microserviço e biblioteca compartilhada do LifeSync.

## Arquivos de Documentação

| Arquivo | Conteúdo |
|---|---|
| [users-service.md](./users-service.md) | Users Service — Autenticação, JWT, Identity, perfil de usuário |
| [taskmanager-service.md](./taskmanager-service.md) | TaskManager Service — Tarefas, labels, lembrete de vencimento |
| [nutrition-service.md](./nutrition-service.md) | Nutrition Service — Diários, refeições, alimentos, líquidos, metas |
| [financial-service.md](./financial-service.md) | Financial Service — Transações, categorias, moedas |
| [gym-service.md](./gym-service.md) | Gym Service — Exercícios, rotinas, sessões de treino |
| [notification-service.md](./notification-service.md) | Notification Service — E-mails via RabbitMQ + SMTP |
| [building-blocks.md](./building-blocks.md) | Building Blocks & Core — CQRS, Result Pattern, Messaging, Domain |

## O que cada documentação contém

- **Visão Geral** — Responsabilidades do serviço
- **Estrutura de Pastas** — Árvore de arquivos
- **Domínio** — Entidades, value objects, enumerações, eventos, erros de domínio
- **Aplicação** — Commands, queries, handlers, validadores, DTOs e contratos de serviço
- **Infraestrutura** — DbContext, repositories, migrations e implementações
- **API** — Todos os endpoints com métodos HTTP, rotas, parâmetros e respostas
- **Configuração** — `appsettings.json` completo
- **Dependências** — Pacotes NuGet e referências internas
