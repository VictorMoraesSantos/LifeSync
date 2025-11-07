# LifeSync

Uma aplicação completa de gerenciamento de vida pessoal construída com arquitetura de microserviços, oferecendo funcionalidades para organização de tarefas, nutrição, finanças pessoais e treinos na academia.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Microserviços](#microserviços)
- [Frontend](#frontend)
- [API Gateway](#api-gateway)
- [Como Executar](#como-executar)
- [Configuração](#configuração)
- [Endpoints da API](#endpoints-da-api)
- [Dashboards](#dashboards)
- [Contribuindo](#contribuindo)
- [Licença](#licença)

## 🎯 Visão Geral

LifeSync é uma plataforma integrada que ajuda os usuários a gerenciar diferentes aspectos de suas vidas em um único lugar:

- **Gerenciamento de Tarefas**: Organize suas tarefas diárias com prioridades, status e labels personalizados
- **Nutrição**: Registre refeições, líquidos e acompanhe seu progresso nutricional diário
- **Financeiro**: Controle receitas, despesas e transações financeiras com categorias personalizadas
- **Academia**: Registre treinos, crie rotinas e acompanhe seu progresso físico
- **Dashboards**: Visualize estatísticas e métricas de cada área com gráficos e relatórios

## 🏗️ Arquitetura

O projeto segue uma arquitetura de **microserviços** baseada em **Clean Architecture** e **Domain-Driven Design (DDD)**, utilizando os seguintes padrões:

- **Separation of Concerns**: Cada microserviço é independente e responsável por um domínio específico
- **CQRS (Command Query Responsibility Segregation)**: Separação entre comandos e consultas
- **API Gateway Pattern**: YARP como gateway único para todas as requisições
- **Event-Driven Architecture**: Comunicação assíncrona via RabbitMQ
- **Repository Pattern**: Abstração da camada de dados

### Diagrama da Arquitetura

```
                        ┌─────────────────┐
                        │  Blazor WebApp  │
                        │  (Frontend)     │
                        └────────┬────────┘
                                 │
                                 │ HTTP/HTTPS
                                 │
                     ┌───────────▼───────────┐
                     │   YARP API Gateway    │
                     │     (Porta 6006)      │
                     └───────────┬───────────┘
                                 │
    ┌─────────────┬──────────────┴─────────────┬────────────┐
    │             │              │             │            │
┌───▼────┐ ┌──────▼──────┐ ┌─────▼─────┐ ┌─────▼─────┐ ┌────▼────┐
│ Users  │ │ TaskManager │ │ Nutrition │ │ Financial │ │  Gym    │
│ Service│ │   Service   │ │  Service  │ │  Service  │ │ Service │
└────────┘ └─────────────┘ └───────────┘ └───────────┘ └─────────┘
    │             │              │             │            │
    │             │              │             │            │
    └─────────────┴──────────────┴─────────────┴────────────┘
                                 │
                           ┌─────▼──────┐
                           │ PostgreSQL │
                           │  Database  │
                           └────────────┘
```

## 🛠️ Tecnologias

### Backend

- **.NET 9.0**: Framework principal
- **ASP.NET Core**: API RESTful
- **Entity Framework Core**: ORM para acesso a dados
- **PostgreSQL**: Banco de dados relacional
- **RabbitMQ**: Message broker para comunicação assíncrona
- **YARP (Yet Another Reverse Proxy)**: API Gateway
- **JWT**: Autenticação e autorização
- **Swagger/OpenAPI**: Documentação da API

### Frontend

- **Blazor WebAssembly**: Framework web interativo
- **Bootstrap 5**: Framework CSS
- **JavaScript/TypeScript**: Para funcionalidades do cliente

### Infraestrutura

- **Docker**: Containerização
- **Docker Compose**: Orquestração de containers
- **MailHog**: Servidor SMTP para desenvolvimento

## 📁 Estrutura do Projeto

```
LifeSync/
├── BuildingBlocks/          # Bibliotecas compartilhadas
│   ├── BuildingBlocks/      # CQRS, Results, Validation
│   └── BuildingBlocks.Messaging/  # RabbitMQ, Events
├── Core/                    # Funcionalidades core compartilhadas
│   ├── Core.API/
│   ├── Core.Application/
│   ├── Core.Domain/
│   └── Core.Infrastructure/
├── Services/                # Microserviços
│   ├── Users/               # Gerenciamento de usuários e autenticação
│   ├── TaskManager/         # Gerenciamento de tarefas
│   ├── Nutrition/           # Gerenciamento nutricional
│   ├── Financial/           # Gerenciamento financeiro
│   ├── Gym/                 # Gerenciamento de treinos
│   ├── Notification/        # Serviço de notificações por email
│   ├── ApiGateways/         # YARP API Gateway
│   └── WebApp/              # Frontend Blazor WebAssembly
└── tests/                   # Testes unitários
```

Cada microserviço segue a estrutura **Clean Architecture**:

```
Service/
├── Service.API/           # Camada de apresentação (Controllers)
├── Service.Application/   # Lógica de negócio (Use Cases, DTOs)
├── Service.Domain/        # Entidades e regras de domínio
└── Service.Infrastructure/ # Implementações (Repositories, External Services)
```

## 🔧 Microserviços

### 1. Users Service

**Responsabilidade**: Gerenciamento de usuários e autenticação

**Funcionalidades**:

- Registro de novos usuários
- Login/Logout
- Recuperação de senha
- Alteração de senha
- Gerenciamento de perfil

**Endpoints**:

- `POST /users-service/api/auth/login`
- `POST /users-service/api/auth/register`
- `POST /users-service/api/auth/logout`
- `POST /users-service/api/auth/forgot-password`
- `POST /users-service/api/auth/reset-password`
- `POST /users-service/api/auth/change-password`
- `GET /users-service/api/users/{id}`
- `PUT /users-service/api/users/{id}`

### 2. TaskManager Service

**Responsabilidade**: Gerenciamento de tarefas e labels

**Funcionalidades**:

- CRUD de tarefas (TaskItems)
- CRUD de labels (TaskLabels)
- Filtros e busca
- Criação em lote
- Prioridades (Baixa, Média, Alta, Urgente)
- Status (Pendente, Em Progresso, Completada, Cancelada)

**Endpoints**:

- `GET /taskmanager-service/api/task-items`
- `GET /taskmanager-service/api/task-items/{id}`
- `GET /taskmanager-service/api/task-items/user/{userId}`
- `POST /taskmanager-service/api/task-items`
- `POST /taskmanager-service/api/task-items/batch`
- `PUT /taskmanager-service/api/task-items/{id}`
- `DELETE /taskmanager-service/api/task-items/{id}`
- `GET /taskmanager-service/api/task-labels`
- `GET /taskmanager-service/api/task-labels/{id}`
- `POST /taskmanager-service/api/task-labels`
- `PUT /taskmanager-service/api/task-labels/{id}`
- `DELETE /taskmanager-service/api/task-labels/{id}`

### 3. Nutrition Service

**Responsabilidade**: Gerenciamento nutricional e acompanhamento alimentar

**Funcionalidades**:

- CRUD de diários nutricionais
- CRUD de refeições (Meals)
- CRUD de alimentos nas refeições (MealFoods)
- CRUD de líquidos (Liquids)
- Progresso diário (DailyProgress)
- Metas diárias de calorias e líquidos

**Endpoints**:

- `GET /nutrition-service/api/diaries`
- `POST /nutrition-service/api/diaries`
- `GET /nutrition-service/api/diaries/{id}`
- `PUT /nutrition-service/api/diaries/{id}`
- `DELETE /nutrition-service/api/diaries/{id}`
- `GET /nutrition-service/api/meals`
- `POST /nutrition-service/api/meals`
- `POST /nutrition-service/api/meals/{mealId}/foods`
- `DELETE /nutrition-service/api/meals/{mealId}/foods/{foodId}`
- `GET /nutrition-service/api/liquids`
- `POST /nutrition-service/api/liquids`
- `GET /nutrition-service/api/daily-progresses`
- `POST /nutrition-service/api/daily-progresses`
- `POST /nutrition-service/api/daily-progresses/{id}/set-goal`

### 4. Financial Service

**Responsabilidade**: Gerenciamento financeiro pessoal

**Funcionalidades**:

- CRUD de transações (Transactions)
- CRUD de categorias (Categories)
- Tipos: Receita e Despesa
- Métodos de pagamento: Dinheiro, Cartão de Crédito/Débito, Transferência, Carteira Digital
- Transações recorrentes
- Relatórios financeiros

**Endpoints**:

- `GET /financial-service/api/transactions`
- `GET /financial-service/api/transactions/{id}`
- `GET /financial-service/api/transactions/user/{userId}`
- `POST /financial-service/api/transactions`
- `PUT /financial-service/api/transactions/{id}`
- `DELETE /financial-service/api/transactions/{id}`
- `GET /financial-service/api/categories`
- `GET /financial-service/api/categories/{id}`
- `POST /financial-service/api/categories`
- `PUT /financial-service/api/categories/{id}`
- `DELETE /financial-service/api/categories/{id}`

### 5. Gym Service

**Responsabilidade**: Gerenciamento de treinos e exercícios

**Funcionalidades**:

- CRUD de exercícios (Exercises)
- CRUD de rotinas (Routines)
- CRUD de sessões de treino (TrainingSessions)
- Exercícios completados (CompletedExercises)
- Exercícios por rotina (RoutineExercises)
- Tipos de exercícios, grupos musculares e equipamentos

**Endpoints**:

- `GET /gym-service/api/exercises`
- `POST /gym-service/api/exercises`
- `GET /gym-service/api/exercises/{id}`
- `PUT /gym-service/api/exercises/{id}`
- `DELETE /gym-service/api/exercises/{id}`
- `GET /gym-service/api/routines`
- `POST /gym-service/api/routines`
- `GET /gym-service/api/routines/{id}`
- `PUT /gym-service/api/routines/{id}`
- `DELETE /gym-service/api/routines/{id}`
- `GET /gym-service/api/training-sessions`
- `POST /gym-service/api/training-sessions`
- `GET /gym-service/api/training-sessions/{id}`
- `PUT /gym-service/api/training-sessions/{id}`
- `DELETE /gym-service/api/training-sessions/{id}`

### 6. Notification Service

**Responsabilidade**: Envio de notificações e emails

**Funcionalidades**:

- Envio de emails
- Processamento de eventos assíncronos
- Templates de email

**Tecnologias**:

- SMTP para envio de emails
- RabbitMQ para consumo de eventos

## 🖥️ Frontend

### Blazor WebAssembly

Aplicação web interativa construída com Blazor WebAssembly que oferece:

- **Interface Responsiva**: Design moderno com Bootstrap 5
- **Autenticação**: Login, registro e gerenciamento de sessão
- **CRUD Completo**: Para todos os módulos (Tarefas, Nutrição, Financeiro, Academia)
- **Dashboards Interativos**: Visualizações de dados e estatísticas
- **LocalStorage**: Persistência local de tokens e dados do usuário

### Páginas Principais

- `/` - Home/Dashboard principal
- `/login` - Página de login
- `/register` - Página de registro
- `/tasks` - Gerenciamento de tarefas
- `/nutrition` - Gerenciamento nutricional
- `/financial` - Gerenciamento financeiro
- `/gym` - Gerenciamento de treinos
- `/dashboard/tasks` - Dashboard de tarefas
- `/dashboard/nutrition` - Dashboard de nutrição
- `/dashboard/financial` - Dashboard financeiro
- `/dashboard/gym` - Dashboard de academia

### Serviços do Frontend

- **AuthService**: Autenticação e gerenciamento de usuários
- **TaskManagerService**: Operações com tarefas
- **NutritionService**: Operações nutricionais
- **FinancialService**: Operações financeiras
- **GymService**: Operações de treinos
- **DashboardService**: Agregação de dados para dashboards

## 🌐 API Gateway

O **YARP (Yet Another Reverse Proxy)** atua como API Gateway único para todos os microserviços:

- **Porta**: `5006` (HTTP) / `5056` (HTTPS)
- **Roteamento**: Baseado em prefixos de caminho
- **Autenticação**: JWT Bearer Token
- **Transformação**: Reescrita de rotas para serviços internos

### Rotas Configuradas

- `/taskmanager-service/*` → TaskManager API
- `/nutrition-service/*` → Nutrition API
- `/financial-service/*` → Financial API
- `/users-service/*` → Users API
- `/gym-service/*` → Gym API

## 🚀 Como Executar

### Pré-requisitos

- **.NET 9.0 SDK**
- **Docker Desktop** (para executar PostgreSQL, RabbitMQ, MailHog)
- **Visual Studio 2022** ou **VS Code** (recomendado)

### Passo 1: Iniciar Infraestrutura

Execute os containers Docker para infraestrutura básica:

```bash
docker-compose up -d lifesyncdb rabbitmq mailhog
```

Isso iniciará:

- **PostgreSQL** na porta `5432`
- **RabbitMQ** com Management UI na porta `15672`
- **MailHog** na porta `1025` (SMTP) e `8025` (Web UI)

### Passo 2: Configurar Banco de Dados

Execute as migrations de cada serviço:

```bash
# Users Service
cd Services/Users/Users.Infrastructure
dotnet ef database update

# TaskManager Service
cd Services/TaskManager/TaskManager.Infrastructure
dotnet ef database update

# Nutrition Service
cd Services/Nutrition/Nutrition.Infrastructure
dotnet ef database update

# Financial Service
cd Services/Financial/Financial.Infrastructure
dotnet ef database update

# Gym Service
cd Services/Gym/Gym.Infrastructure
dotnet ef database update

# Notification Service
cd Services/Notification/Notification.Infrastructure
dotnet ef database update
```

### Passo 3: Executar os Microserviços

Execute cada serviço em terminais separados:

```bash
# API Gateway (deve ser executado primeiro)
cd Services/ApiGateways/YarpApiGateway
dotnet run

# Users Service
cd Services/Users/Users.API
dotnet run

# TaskManager Service
cd Services/TaskManager/TaskManager.API
dotnet run

# Nutrition Service
cd Services/Nutrition/Nutrition.API
dotnet run

# Financial Service
cd Services/Financial/Financial.API
dotnet run

# Gym Service
cd Services/Gym/Gym.API
dotnet run

# Notification Service
cd Services/Notification/Notification.API
dotnet run
```

### Passo 4: Executar Frontend

```bash
cd Services/WebApp/LifeSyncApp/LifeSyncApp
dotnet run
```

### Portas dos Serviços

| Serviço          | Porta HTTP | Porta HTTPS |
| ---------------- | ---------- | ----------- |
| API Gateway      | 5006       | 5056        |
| Users API        | 5001       | 7001        |
| TaskManager API  | 5002       | 7002        |
| Nutrition API    | 5003       | 7003        |
| Financial API    | 5004       | 7004        |
| Gym API          | 5005       | 7005        |
| Notification API | 5126       | 7012        |
| Blazor WebApp    | 5068       | 7124        |

### Executar com Docker Compose

Para executar todos os serviços via Docker Compose:

```bash
docker-compose up --build
```

## ⚙️ Configuração

### Connection Strings

Cada serviço possui seu próprio `appsettings.json` com connection strings. Configure conforme necessário:

```json
{
  "ConnectionStrings": {
    "Database": "Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=LifeSync;Include Error Detail=true;"
  }
}
```

### JWT Settings

Configure no API Gateway (`Services/ApiGateways/YarpApiGateway/appsettings.json`):

```json
{
  "JwtSettings": {
    "Key": "your_very_long_secret_key_here_which_should_be_at_least_32_chars",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience",
    "ExpiryMinutes": 120,
    "RefreshTokenExpiryDays": 7
  }
}
```

### RabbitMQ Settings

Configure em cada serviço que utiliza RabbitMQ:

```json
{
  "RabbitMQSettings": {
    "Host": "localhost",
    "User": "guest",
    "Password": "guest",
    "Port": 5672
  }
}
```

### Frontend API Configuration

Configure no `Services/WebApp/LifeSyncApp/LifeSyncApp.Client/wwwroot/appsettings.json`:

```json
{
  "ApiBaseUrl": "http://localhost:5006"
}
```

## 📊 Dashboards

O sistema inclui dashboards interativos para cada microserviço:

### Dashboard de Tarefas (`/dashboard/tasks`)

- Total de tarefas, completadas, pendentes, em progresso
- Distribuição por status e prioridade
- Atividade recente (últimos 7 dias)

### Dashboard de Nutrição (`/dashboard/nutrition`)

- Total de diários, refeições e líquidos
- Média de calorias e líquidos por dia
- Tendência de calorias (últimos 14 dias)
- Top refeições por calorias

### Dashboard Financeiro (`/dashboard/financial`)

- Total de receitas, despesas e saldo líquido
- Tendência mensal (últimos 6 meses)
- Gastos por categoria
- Distribuição por método de pagamento

### Dashboard de Academia (`/dashboard/gym`)

- Total de sessões, rotinas e exercícios
- Sessões do mês e duração média
- Tendência semanal (últimas 4 semanas)
- Uso de rotinas e frequência de exercícios

## 🧪 Testes

Execute os testes unitários:

```bash
cd tests/TaskManager.UnitTests
dotnet test
```

## 📝 Padrões de Código

### CQRS Pattern

Cada serviço utiliza CQRS para separação de comandos e consultas:

- **Commands**: Operações de escrita (Create, Update, Delete)
- **Queries**: Operações de leitura (Get, GetAll, Search)
- **Handlers**: Processamento de comandos e consultas

### Result Pattern

Todas as operações retornam um `Result<T>` ou `HttpResult<T>`:

```csharp
public class HttpResult<T>
{
    public bool Success { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string[] Errors { get; set; }
    public T? Data { get; set; }
    public PaginationData? Pagination { get; set; }
}
```

### Validation

Validação automática usando FluentValidation integrado ao pipeline CQRS.

## 🔒 Segurança

- **JWT Authentication**: Tokens Bearer para autenticação
- **HTTPS**: Suportado em todos os serviços
- **CORS**: Configurado para desenvolvimento
- **Authorization Policies**: Aplicadas via API Gateway

## 📚 Documentação da API

Cada serviço expõe documentação Swagger/OpenAPI:

- TaskManager: `http://localhost:5002/swagger`
- Nutrition: `http://localhost:5003/swagger`
- Financial: `http://localhost:5004/swagger`
- Users: `http://localhost:5001/swagger`
- Gym: `http://localhost:5005/swagger`

## 🤝 Contribuindo

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

### Padrões de Commits

- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Documentação
- `style`: Formatação
- `refactor`: Refatoração
- `test`: Testes
- `chore`: Manutenção

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE.txt` para mais detalhes.

## 👥 Autores

- **Victor Moraes** - _Desenvolvimento Inicial_

## 🙏 Agradecimentos

- .NET Community
- Blazor Community
- Todos os contribuidores de open source que tornaram este projeto possível

---

**LifeSync** - Organize sua vida em um único lugar! 🚀
