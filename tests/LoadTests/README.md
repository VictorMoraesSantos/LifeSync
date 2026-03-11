# LifeSync - Testes de Carga (Load Tests)

Suite completa de testes de carga para todos os microserviços do projeto **LifeSync**, implementada com [k6](https://k6.io/) (Grafana k6) — uma ferramenta moderna de load testing escrita em Go que executa scripts JavaScript.

---

## Sumário

1. [Visão Geral](#1-visão-geral)
2. [Arquitetura dos Testes](#2-arquitetura-dos-testes)
3. [Estrutura de Arquivos](#3-estrutura-de-arquivos)
4. [Pré-requisitos](#4-pré-requisitos)
5. [Biblioteca Compartilhada (`lib/`)](#5-biblioteca-compartilhada-lib)
6. [Cenários de Teste por Microserviço](#6-cenários-de-teste-por-microserviço)
7. [Perfis de Carga](#7-perfis-de-carga)
8. [Thresholds (Critérios de Aprovação)](#8-thresholds-critérios-de-aprovação)
9. [Comandos de Execução](#9-comandos-de-execução)
10. [Variáveis de Ambiente](#10-variáveis-de-ambiente)
11. [Exportação e Visualização de Resultados](#11-exportação-e-visualização-de-resultados)
12. [Mapa Completo de Endpoints Testados](#12-mapa-completo-de-endpoints-testados)
13. [Considerações e Boas Práticas](#13-considerações-e-boas-práticas)

---

## 1. Visão Geral

Os testes de carga foram projetados para validar a performance, estabilidade e resiliência de todos os microserviços do LifeSync sob diferentes condições de tráfego. A suite cobre:

- **5 microserviços** testados individualmente (Users, TaskManager, Financial, Gym, Nutrition)
- **1 cenário de API Gateway** que testa o roteamento cross-service pelo YARP
- **6 perfis de carga** (smoke, low, average, stress, spike, soak)
- **60+ endpoints** cobertos com operações CRUD completas
- **Autenticação JWT** integrada em todos os cenários
- **Cleanup automático** — cada iteração cria e remove seus próprios dados
- **Modo gateway** — possibilidade de rotear todos os testes pelo YARP API Gateway

### Ferramenta Utilizada: k6

O **k6** foi escolhido por:
- Scripting em JavaScript ES6 (familiar para desenvolvedores)
- Alta performance (engine em Go, não Node.js)
- Métricas built-in (latência, throughput, taxa de erro, percentis)
- Thresholds nativos para aprovação/reprovação automática
- Integração com Grafana, InfluxDB, Prometheus, Datadog
- Suporte a cenários ramping com Virtual Users (VUs)

---

## 2. Arquitetura dos Testes

```
┌─────────────────────────────────────────────────────────────┐
│                    k6 Load Test Runner                       │
│                                                             │
│  ┌─────────┐  ┌────────────┐  ┌───────────┐  ┌───────────┐ │
│  │  smoke   │  │   average  │  │  stress   │  │   spike   │ │
│  │  1 VU    │  │   50 VUs   │  │  300 VUs  │  │  500 VUs  │ │
│  └────┬─────┘  └─────┬──────┘  └─────┬─────┘  └─────┬─────┘ │
│       │              │               │               │       │
│       └──────────────┴───────────────┴───────────────┘       │
│                           │                                  │
│               ┌───────────┴───────────┐                      │
│               │   setup(): Login JWT  │                      │
│               └───────────┬───────────┘                      │
│                           │                                  │
│          ┌────────────────┼────────────────┐                 │
│          ▼                ▼                ▼                  │
│   ┌─────────────┐ ┌─────────────┐ ┌─────────────┐          │
│   │   CREATE    │ │  READ/LIST  │ │   UPDATE    │          │
│   │  (POST)     │ │  (GET)      │ │  (PUT)      │          │
│   └──────┬──────┘ └─────────────┘ └──────┬──────┘          │
│          │                               │                   │
│          └───────────┬───────────────────┘                   │
│                      ▼                                       │
│              ┌─────────────┐                                 │
│              │   DELETE    │  ← Cleanup automático           │
│              │  (DELETE)   │                                  │
│              └─────────────┘                                 │
└─────────────────────────────────────────────────────────────┘
                       │
         ┌─────────────┼─────────────────────────┐
         ▼             ▼                         ▼
  ┌──────────┐  ┌──────────────┐  ┌────────────────────┐
  │  Direto  │  │   Via YARP   │  │  Docker (8080)     │
  │ :5000-04 │  │   Gateway    │  │  Container ports   │
  │          │  │   :5006      │  │                    │
  └──────────┘  └──────────────┘  └────────────────────┘
```

### Fluxo de Cada Cenário

1. **`setup()`** — Executado uma única vez antes do teste. Autentica o usuário de teste no Users Service e retorna o JWT token.
2. **`default function(data)`** — Executado em loop por cada Virtual User (VU). Recebe o token do setup. Realiza operações CRUD com dados randomizados.
3. **Cleanup** — Ao final de cada iteração, os recursos criados são deletados para não poluir o banco.
4. **Thresholds** — Após o término, o k6 verifica se as métricas atendem aos critérios definidos.

---

## 3. Estrutura de Arquivos

```
tests/LoadTests/
│
├── lib/                           # Biblioteca compartilhada
│   ├── config.js                  # URLs, perfis, thresholds, gateway config
│   └── helpers.js                 # Auth, HTTP checks, geradores de dados
│
├── scenarios/                     # Cenários de teste (1 por microserviço)
│   ├── users.test.js              # Users Service — auth + users
│   ├── taskmanager.test.js        # TaskManager — tasks + labels (CRUD completo)
│   ├── financial.test.js          # Financial — transactions + categories (CRUD completo)
│   ├── gym.test.js                # Gym — routines, exercises, sessions (CRUD completo)
│   ├── nutrition.test.js          # Nutrition — diaries, meals, foods, liquids (CRUD completo)
│   └── gateway.test.js            # YARP Gateway — routing cross-service
│
├── results/                       # Resultados gerados (gitignored)
├── run-tests.sh                   # Script runner para execução em batch
├── .gitignore                     # Ignora pasta results/
└── README.md                      # Esta documentação
```

---

## 4. Pré-requisitos

### 4.1 Instalar o k6

**Windows (winget):**
```bash
winget install k6 --source winget
```

**Windows (Chocolatey):**
```bash
choco install k6
```

**Windows (Scoop):**
```bash
scoop install k6
```

**macOS (Homebrew):**
```bash
brew install k6
```

**Linux (Debian/Ubuntu):**
```bash
sudo gpg -k
sudo gpg --no-default-keyring --keyring /usr/share/keyrings/k6-archive-keyring.gpg \
  --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys C5AD17C747E3415A3642D57D77C6C491D6AC1D69
echo "deb [signed-by=/usr/share/keyrings/k6-archive-keyring.gpg] https://dl.k6.io/deb stable main" | \
  sudo tee /etc/apt/sources.list.d/k6.list
sudo apt-get update && sudo apt-get install k6
```

**Docker:**
```bash
docker pull grafana/k6
```

**Verificar instalação:**
```bash
k6 version
```

### 4.2 Serviços em Execução

Os microserviços devem estar rodando antes de executar os testes. As portas padrão são:

| Serviço          | HTTP Local | HTTPS Local | Docker (HTTP) |
|------------------|-----------|-------------|---------------|
| TaskManager      | 5000      | 5050        | 8080          |
| Users            | 5001      | 5051        | 8080          |
| Nutrition        | 5002      | 5052        | 8080          |
| Financial        | 5003      | 5053        | 8080          |
| Gym              | 5004      | 5054        | 8080          |
| Notification     | 5126      | 7012        | 8080          |
| YARP API Gateway | 5006      | 5056        | 8080          |

### 4.3 Usuário de Teste

Um usuário deve existir (ou o cenário `users.test.js` tenta criar um automaticamente). Credenciais padrão:

- **Email:** `loadtest@test.com`
- **Senha:** `LoadTest@123`

Essas credenciais podem ser alteradas via variáveis de ambiente (ver [seção 10](#10-variáveis-de-ambiente)).

---

## 5. Biblioteca Compartilhada (`lib/`)

### 5.1 `config.js` — Configuração Central

Este arquivo centraliza toda a configuração dos testes:

**URLs dos serviços** (`SERVICES`):
Objeto com as URLs base de cada microserviço. Cada URL pode ser sobrescrita por variável de ambiente (`GATEWAY_URL`, `USERS_URL`, `TASKMANAGER_URL`, `FINANCIAL_URL`, `GYM_URL`, `NUTRITION_URL`, `NOTIFICATION_URL`).

**Modo Gateway** (`USE_GATEWAY`):
Quando `true`, a função `baseUrl(service)` retorna a URL do gateway com o prefixo do serviço correspondente ao invés da URL direta. Prefixos do YARP:

| Serviço     | Prefixo Gateway           |
|-------------|---------------------------|
| Users       | `/users-service`          |
| Auth        | `/auth`                   |
| TaskManager | `/taskmanager-service`    |
| Financial   | `/financial-service`      |
| Gym         | `/gym-service`            |
| Nutrition   | `/nutrition-service`      |

**Perfis de carga** (`PROFILES`):
6 perfis pré-configurados (detalhados na [seção 7](#7-perfis-de-carga)).

**Thresholds** (`DEFAULT_THRESHOLDS`, `STRICT_THRESHOLDS`):
Critérios de aprovação (detalhados na [seção 8](#8-thresholds-critérios-de-aprovação)).

**`getProfile(defaultProfile)`**:
Função que lê o perfil da variável de ambiente `K6_PROFILE` ou usa o default passado como argumento.

### 5.2 `helpers.js` — Funções Utilitárias

| Função | Descrição |
|--------|-----------|
| `jsonHeaders(token?)` | Retorna headers `Content-Type: application/json` com `Authorization: Bearer` opcional |
| `authenticate(url, email, password)` | Faz POST no `/api/auth/login`, valida resposta e retorna o `accessToken` |
| `registerUser(url, userData)` | Faz POST no `/api/auth/register` com `firstName`, `lastName`, `email`, `password` |
| `uniqueEmail(prefix?)` | Gera email único usando timestamp + random: `{prefix}_{ts}_{rand}@test.com` |
| `randomInt(min, max)` | Retorna inteiro aleatório entre min e max (inclusivo) |
| `randomChoice(array)` | Retorna elemento aleatório de um array |
| `futureDate(daysAhead?)` | Gera data futura no formato `YYYY-MM-DD` (1 a N dias à frente, default 30) |
| `pastDateTime(daysBack?)` | Gera datetime ISO no passado (0 a N dias atrás, default 30) |
| `checkCreated(res, label)` | Verifica se o status é `201 Created` |
| `checkOk(res, label)` | Verifica se o status é `200 OK` |
| `checkSuccess(res, label)` | Verifica se o status está na faixa `2xx` |

---

## 6. Cenários de Teste por Microserviço

### 6.1 Users Service (`scenarios/users.test.js`)

**Arquivo:** `scenarios/users.test.js`
**Tag:** `service: users`
**URL base:** `http://localhost:5001`

**Setup:**
- Registra um usuário de teste (ignora se já existir)
- Autentica e retorna o JWT token

**Grupos de teste por iteração (por VU):**

| Grupo | Método | Endpoint | Descrição |
|-------|--------|----------|-----------|
| `Auth - Register` | POST | `/api/auth/register` | Registra um novo usuário com email único gerado por `uniqueEmail('reg')`. Body: `{ firstName, lastName, email, password }` |
| `Auth - Login` | POST | `/api/auth/login` | Faz login com as credenciais do usuário de teste. Body: `{ email, password }` |
| `Users - Get All` | GET | `/api/users` | Lista todos os usuários (requer JWT) |

**Dados gerados:**
- Email: `reg_{timestamp}_{random}@test.com`
- Senha fixa: `Test@12345`

**Sleep entre operações:** 0.3s - 0.5s

---

### 6.2 TaskManager Service (`scenarios/taskmanager.test.js`)

**Arquivo:** `scenarios/taskmanager.test.js`
**Tag:** `service: taskmanager`
**URL base:** `http://localhost:5000`

**Setup:**
- Autentica no Users Service e retorna o token

**Enums utilizados:**
- **Priority:** `1` (Low), `2` (Medium), `3` (High), `4` (Urgent)
- **Status:** `1` (Pending), `2` (InProgress), `3` (Completed)
- **LabelColor:** `0` (Red) até `8` (Gray)

**Grupos de teste por iteração (por VU):**

| # | Grupo | Método | Endpoint | Body / Params |
|---|-------|--------|----------|---------------|
| 1 | `TaskLabels - Create` | POST | `/api/task-labels` | `{ name: "Label_{ts}_{rand}", labelColor: randomInt(0,8) }` |
| 2 | `TaskLabels - Get All` | GET | `/api/task-labels` | — |
| 3 | `TaskItems - Create` | POST | `/api/task-items` | `{ title, description, priority: random(1-4), dueDate: futuro(30d), taskLabelsId: [labelId] }` |
| 4 | `TaskItems - Get All` | GET | `/api/task-items` | — |
| 5 | `TaskItems - Search` | GET | `/api/task-items/search?priority=2&status=1` | Query params |
| 6 | `TaskItems - Get By Id` | GET | `/api/task-items/{id}` | ID do task criado |
| 7 | `TaskItems - Update` | PUT | `/api/task-items/{id}` | `{ title, description, status: random(1-3), priority: random(1-4), dueDate: futuro(60d) }` |
| 8 | `TaskItems - Delete` | DELETE | `/api/task-items/{id}` | ID do task criado |
| 9 | `TaskLabels - Delete` | DELETE | `/api/task-labels/{id}` | ID do label criado |

**Fluxo de dependência:** Create Label → Create Task (com labelId) → Read/Search → Update → Delete Task → Delete Label

---

### 6.3 Financial Service (`scenarios/financial.test.js`)

**Arquivo:** `scenarios/financial.test.js`
**Tag:** `service: financial`
**URL base:** `http://localhost:5003`

**Setup:**
- Autentica no Users Service e retorna o token

**Enums utilizados:**
- **PaymentMethod:** `1` (Cash), `2` (CreditCard), `3` (DebitCard), `4` (BankTransfer), `5` (Pix), `6` (Other)
- **TransactionType:** `1` (Income), `2` (Expense)

**Grupos de teste por iteração (por VU):**

| # | Grupo | Método | Endpoint | Body / Params |
|---|-------|--------|----------|---------------|
| 1 | `Categories - Create` | POST | `/api/categories` | `{ name: "Cat_{ts}_{rand}", description }` |
| 2 | `Categories - Get All` | GET | `/api/categories` | — |
| 3 | `Categories - Search` | GET | `/api/categories/search?name=Cat` | Query param |
| 4 | `Transactions - Create` | POST | `/api/transactions` | `{ categoryId, paymentMethod: random(1-6), transactionType: random(1-2), amount: { amount: random(10-5000), currency: "BRL" }, description, transactionDate: passado(30d), isRecurring: false }` |
| 5 | `Transactions - Get All` | GET | `/api/transactions` | — |
| 6 | `Transactions - Search` | GET | `/api/transactions/search?transactionType=1` | Query param |
| 7 | `Transactions - Get By Id` | GET | `/api/transactions/{id}` | ID da transação criada |
| 8 | `Transactions - Update` | PUT | `/api/transactions/{id}` | Dados atualizados com novos valores random |
| 9 | `Transactions - Delete` | DELETE | `/api/transactions/{id}` | — |
| 10 | `Categories - Delete` | DELETE | `/api/categories/{id}` | Cleanup |

**Fluxo de dependência:** Create Category → Create Transaction (com categoryId) → Read/Search → Update → Delete Transaction → Delete Category

---

### 6.4 Gym Service (`scenarios/gym.test.js`)

**Arquivo:** `scenarios/gym.test.js`
**Tag:** `service: gym`
**URL base:** `http://localhost:5004`

**Setup:**
- Autentica no Users Service e retorna o token

**Enums utilizados:**
- **MuscleGroup:** `Chest`, `Back`, `Shoulders`, `Biceps`, `Triceps`, `Forearms`, `Abs`, `Quadriceps`, `Hamstrings`, `Calves`, `Glutes`, `LowerBack`, `Traps`, `Core`
- **ExerciseType:** `Strength`, `Hypertrophy`, `Endurance`, `Power`, `Flexibility`, `Cardio`, `HIIT`, `Recovery`
- **EquipmentType:** `Barbell`, `Dumbbell`, `Machine`, `Cable`, `Bodyweight`, `ResistanceBand`, `Kettlebell`, `Bench`

**Grupos de teste por iteração (por VU):**

| # | Grupo | Método | Endpoint | Body / Params |
|---|-------|--------|----------|---------------|
| 1 | `Routines - Create` | POST | `/api/routines` | `{ name: "Routine_{ts}_{rand}", description }` |
| 2 | `Routines - Get All` | GET | `/api/routines` | — |
| 3 | `Routines - Search` | GET | `/api/routines/search?name=Routine` | Query param |
| 4 | `Exercises - Create` | POST | `/api/exercises` | `{ name, description, muscleGroup: random, exerciseType: random, equipmentType: random }` |
| 5 | `Exercises - Get All` | GET | `/api/exercises` | — |
| 6 | `RoutineExercises - Create` | POST | `/api/routine-exercises` | `{ routineId, exerciseId, sets: random(2-5), repetitions: random(6-15), restBetweenSets: random(30-120), recommendedWeight: random(10-100), instructions }` |
| 7 | `RoutineExercises - Get All` | GET | `/api/routine-exercises` | — |
| 8 | `TrainingSessions - Create` | POST | `/api/training-sessions` | `{ routineId, startTime: 1h atrás, endTime: agora, notes }` |
| 9 | `TrainingSessions - Get All` | GET | `/api/training-sessions` | — |
| — | Cleanup | DELETE | Todos os endpoints acima | Deleta session → routine-exercise → exercise → routine |

**Fluxo de dependência:** Create Routine → Create Exercise → Create RoutineExercise (vincula ambos) → Create TrainingSession → Read All → Cleanup em ordem reversa

---

### 6.5 Nutrition Service (`scenarios/nutrition.test.js`)

**Arquivo:** `scenarios/nutrition.test.js`
**Tag:** `service: nutrition`
**URL base:** `http://localhost:5002`

**Setup:**
- Autentica no Users Service e retorna o token

**Grupos de teste por iteração (por VU):**

| # | Grupo | Método | Endpoint | Body / Params |
|---|-------|--------|----------|---------------|
| 1 | `Diaries - Create` | POST | `/api/diaries` | `{ date: futureDate(30) }` |
| 2 | `Diaries - Get All` | GET | `/api/diaries` | — |
| 3 | `Meals - Create` | POST | `/api/meals` | `{ diaryId, name: "Meal_{ts}_{rand}", description }` |
| 4 | `Meals - Get All` | GET | `/api/meals` | — |
| 5 | `Meals - Get By Diary` | GET | `/api/meals/diary/{diaryId}` | — |
| 6 | `Foods - Get All` | GET | `/api/foods` | — (dados pré-seedados) |
| 7 | `Foods - Search` | GET | `/api/foods/search?name=rice` | Query param |
| 8 | `DailyProgresses - Create` | POST | `/api/daily-progresses` | `{ caloriesConsumed: random(500-3000), liquidsConsumedMl: random(500-3000), goal: random(1500-2500) }` |
| 9 | `DailyProgresses - Get All` | GET | `/api/daily-progresses` | — |
| 10 | `LiquidTypes - Create` | POST | `/api/liquid-types` | `{ name: "Liquid_{ts}_{rand}" }` |
| 11 | `LiquidTypes - Get All` | GET | `/api/liquid-types` | — |
| 12 | `Liquids - Create` | POST | `/api/liquids` | `{ liquidTypeId, quantity: random(100-500) }` |
| — | Cleanup | DELETE | Vários | Deleta liquid → progress → meal → liquidType → diary |

**Fluxo de dependência:** Create Diary → Create Meal (com diaryId) + Create LiquidType → Create Liquid (com liquidTypeId) → Reads → Cleanup em ordem reversa

---

### 6.6 YARP API Gateway (`scenarios/gateway.test.js`)

**Arquivo:** `scenarios/gateway.test.js`
**Tag:** `service: gateway`
**URL base:** `http://localhost:5006` (sempre usa o gateway diretamente)

**Objetivo:** Medir o overhead de roteamento do YARP e validar que todos os microserviços são acessíveis pelo gateway.

**Setup:**
- Faz login via rota do gateway (`/auth/login`) e retorna o token

**Grupos de teste por iteração (por VU):**

| # | Grupo | Método | Rota Gateway | Serviço Destino |
|---|-------|--------|-------------|-----------------|
| 1 | `Gateway - Auth Login` | POST | `/auth/login` | Users → `/api/auth/login` |
| 2 | `Gateway - TaskManager: Get Tasks` | GET | `/taskmanager-service/api/task-items` | TaskManager → `/api/task-items` |
| 3 | `Gateway - TaskManager: Create Task` | POST | `/taskmanager-service/api/task-items` | TaskManager → `/api/task-items` |
| 4 | `Gateway - Financial: Get Transactions` | GET | `/financial-service/api/transactions` | Financial → `/api/transactions` |
| 5 | `Gateway - Financial: Get Categories` | GET | `/financial-service/api/categories` | Financial → `/api/categories` |
| 6 | `Gateway - Gym: Get Routines` | GET | `/gym-service/api/routines` | Gym → `/api/routines` |
| 7 | `Gateway - Gym: Get Exercises` | GET | `/gym-service/api/exercises` | Gym → `/api/exercises` |
| 8 | `Gateway - Nutrition: Get Diaries` | GET | `/nutrition-service/api/diaries` | Nutrition → `/api/diaries` |
| 9 | `Gateway - Nutrition: Get Foods` | GET | `/nutrition-service/api/foods` | Nutrition → `/api/foods` |
| 10 | `Gateway - Users: Get Users` | GET | `/users-service/api/users` | Users → `/api/users` |

**Thresholds relaxados:** p(95) < 800ms e p(99) < 2000ms (devido ao overhead de roteamento do reverse proxy)

---

## 7. Perfis de Carga

### 7.1 Smoke (`smoke`)
**Objetivo:** Verificação básica de que os endpoints estão respondendo corretamente.
```
1 VU constante por 30 segundos
```
- Usar como primeiro teste para validar que tudo funciona antes de escalar.

### 7.2 Low (`low`)
**Objetivo:** Estabelecer uma baseline de performance com carga mínima.
```
0s-30s   → Ramp-up de 0 para 10 VUs
30s-1m30 → Manutenção de 10 VUs
1m30-2m  → Ramp-down para 0 VUs
```
- Duração total: ~2 minutos

### 7.3 Average (`average`) — **PADRÃO**
**Objetivo:** Simular o uso típico do sistema em condições normais.
```
0s-1m    → Ramp-up de 0 para 50 VUs
1m-4m    → Manutenção de 50 VUs
4m-5m    → Ramp-down para 0 VUs
```
- Duração total: ~5 minutos
- Este é o perfil padrão quando nenhum é especificado.

### 7.4 Stress (`stress`)
**Objetivo:** Encontrar os limites do sistema aumentando progressivamente a carga.
```
0s-1m    → Ramp-up de 0 para 50 VUs
1m-3m    → Aumento para 100 VUs
3m-5m    → Aumento para 200 VUs
5m-7m    → Aumento para 300 VUs
7m-9m    → Ramp-down para 0 VUs
```
- Duração total: ~9 minutos
- Observar em qual estágio os thresholds começam a falhar.

### 7.5 Spike (`spike`)
**Objetivo:** Testar a resiliência a rajadas súbitas de tráfego (picos inesperados).
```
0s-30s   → Aquecimento com 10 VUs
30s-40s  → SPIKE para 500 VUs (em 10 segundos!)
40s-1m40 → Manutenção de 500 VUs
1m40-1m50 → Drop para 10 VUs
1m50-2m20 → Ramp-down para 0 VUs
```
- Duração total: ~2.5 minutos
- Testa a capacidade de auto-scaling e recuperação.

### 7.6 Soak (`soak`)
**Objetivo:** Detectar memory leaks, degradação gradual e problemas de estabilidade a longo prazo.
```
0s-2m    → Ramp-up de 0 para 100 VUs
2m-32m   → Manutenção de 100 VUs por 30 minutos
32m-34m  → Ramp-down para 0 VUs
```
- Duração total: ~34 minutos
- Monitorar uso de memória e CPU do servidor durante o teste.

---

## 8. Thresholds (Critérios de Aprovação)

### 8.1 Thresholds Padrão (`DEFAULT_THRESHOLDS`)

Aplicados aos cenários `users`, `taskmanager`, `financial`, `gym` e `nutrition`:

| Métrica | Critério | Descrição |
|---------|----------|-----------|
| `http_req_duration` | `p(95) < 500ms` | 95% das requisições devem completar em menos de 500ms |
| `http_req_duration` | `p(99) < 1500ms` | 99% das requisições devem completar em menos de 1.5s |
| `http_req_failed` | `rate < 0.05` | Menos de 5% das requisições podem falhar |
| `http_reqs` | `rate > 10` | Throughput mínimo de 10 requisições por segundo |

### 8.2 Thresholds Estritos (`STRICT_THRESHOLDS`)

Disponíveis para uso em cenários que exigem alta performance:

| Métrica | Critério |
|---------|----------|
| `http_req_duration` | `p(95) < 200ms` |
| `http_req_duration` | `p(99) < 500ms` |
| `http_req_failed` | `rate < 0.01` (< 1% erro) |
| `http_reqs` | `rate > 50` rps |

### 8.3 Thresholds do Gateway

O cenário `gateway.test.js` usa thresholds relaxados:

| Métrica | Critério | Justificativa |
|---------|----------|---------------|
| `http_req_duration` | `p(95) < 800ms` | Overhead de roteamento YARP |
| `http_req_duration` | `p(99) < 2000ms` | Latência adicional do proxy reverso |
| `http_req_failed` | `rate < 0.05` | Mesmo critério padrão |
| `http_reqs` | `rate > 10` | Mesmo critério padrão |

### Interpretação dos Resultados

Após a execução, o k6 exibe um resumo com todas as métricas. Se algum threshold falhar, o k6 retorna exit code `99` e marca o threshold com `✗`:

```
✓ http_req_duration....: avg=120ms  min=15ms  med=95ms  max=2.1s   p(95)=350ms  p(99)=890ms
✗ http_req_failed......: 7.23%  ✗ 852 / 11784
  ↳  rate<0.05.........: fail (7.23% > 5.00%)
✓ http_reqs............: 39.28/s
```

---

## 9. Comandos de Execução

### 9.1 Execução Direta com k6

**Navegar até a pasta dos testes:**
```bash
cd C:/Users/Victor/source/repos/LifeSync/tests/LoadTests
```

**Smoke test (validação rápida — comece por aqui):**
```bash
# Users Service
k6 run --env K6_PROFILE=smoke scenarios/users.test.js

# TaskManager Service
k6 run --env K6_PROFILE=smoke scenarios/taskmanager.test.js

# Financial Service
k6 run --env K6_PROFILE=smoke scenarios/financial.test.js

# Gym Service
k6 run --env K6_PROFILE=smoke scenarios/gym.test.js

# Nutrition Service
k6 run --env K6_PROFILE=smoke scenarios/nutrition.test.js

# API Gateway
k6 run --env K6_PROFILE=smoke scenarios/gateway.test.js
```

**Carga média (perfil padrão — uso típico):**
```bash
k6 run scenarios/taskmanager.test.js
# ou explicitamente:
k6 run --env K6_PROFILE=average scenarios/taskmanager.test.js
```

**Stress test (encontrar limites):**
```bash
k6 run --env K6_PROFILE=stress scenarios/taskmanager.test.js
```

**Spike test (rajada de tráfego):**
```bash
k6 run --env K6_PROFILE=spike scenarios/financial.test.js
```

**Soak test (longa duração — detecção de leak):**
```bash
k6 run --env K6_PROFILE=soak scenarios/gym.test.js
```

### 9.2 Roteamento pelo API Gateway

Redireciona todas as requisições de qualquer cenário pelo YARP API Gateway:

```bash
# TaskManager via gateway
k6 run --env USE_GATEWAY=true --env K6_PROFILE=average scenarios/taskmanager.test.js

# Financial via gateway com stress
k6 run --env USE_GATEWAY=true --env K6_PROFILE=stress scenarios/financial.test.js

# Todos via gateway (usando o runner script)
USE_GATEWAY=true ./run-tests.sh all average
```

### 9.3 Credenciais Customizadas

```bash
k6 run \
  --env TEST_EMAIL=meuuser@email.com \
  --env TEST_PASSWORD=MinhaSenh@Forte123 \
  --env K6_PROFILE=average \
  scenarios/taskmanager.test.js
```

### 9.4 URLs Customizadas (ambiente remoto)

```bash
# Apontar para servidor staging
k6 run \
  --env USERS_URL=http://staging.lifesync.com:5001 \
  --env TASKMANAGER_URL=http://staging.lifesync.com:5000 \
  --env K6_PROFILE=low \
  scenarios/taskmanager.test.js

# Apontar para API Gateway de staging
k6 run \
  --env GATEWAY_URL=http://staging.lifesync.com:5006 \
  --env K6_PROFILE=average \
  scenarios/gateway.test.js
```

### 9.5 Usando o Script Runner (`run-tests.sh`)

```bash
# Dar permissão de execução (uma vez)
chmod +x run-tests.sh

# Rodar TODOS os cenários com perfil padrão (average)
./run-tests.sh

# Rodar apenas um serviço
./run-tests.sh taskmanager
./run-tests.sh financial
./run-tests.sh gym
./run-tests.sh nutrition
./run-tests.sh users
./run-tests.sh gateway

# Rodar todos com perfil específico
./run-tests.sh all smoke
./run-tests.sh all stress
./run-tests.sh all spike
./run-tests.sh all soak

# Rodar um serviço com perfil específico
./run-tests.sh taskmanager stress
./run-tests.sh gateway spike

# Combinar com variáveis de ambiente
USE_GATEWAY=true TEST_EMAIL=admin@test.com ./run-tests.sh all average
```

**Saída do runner:**
```
============================================
 LifeSync Load Tests
 Profile: average
 Target:  all
 Time:    Mon Mar 10 14:30:22 2026
============================================

>>> Running: financial (average profile)
-------------------------------------------
[...output do k6...]

>>> Completed: financial
    Results: results/financial_average_20260310_143022.log
===========================================

>>> Running: gateway (average profile)
-------------------------------------------
[...output do k6...]
```

Os resultados são salvos em `results/` com o padrão: `{cenário}_{perfil}_{timestamp}.log` e `{cenário}_{perfil}_{timestamp}.json`.

### 9.6 Via Docker

```bash
# Execução simples (rede host)
docker run --rm -i \
  --network host \
  -v "$(pwd):/tests" \
  grafana/k6 run /tests/scenarios/taskmanager.test.js

# Com variáveis de ambiente
docker run --rm -i \
  --network host \
  -v "$(pwd):/tests" \
  -e K6_PROFILE=stress \
  -e TEST_EMAIL=loadtest@test.com \
  -e TEST_PASSWORD=LoadTest@123 \
  grafana/k6 run /tests/scenarios/taskmanager.test.js

# Docker Compose (se os serviços estão em docker-compose)
docker run --rm -i \
  --network lifesync_default \
  -v "$(pwd):/tests" \
  -e USERS_URL=http://users.api:8080 \
  -e TASKMANAGER_URL=http://taskmanager.api:8080 \
  grafana/k6 run /tests/scenarios/taskmanager.test.js
```

---

## 10. Variáveis de Ambiente

| Variável | Tipo | Padrão | Descrição |
|----------|------|--------|-----------|
| `K6_PROFILE` | string | `average` | Perfil de carga: `smoke`, `low`, `average`, `stress`, `spike`, `soak` |
| `USE_GATEWAY` | boolean | `false` | Quando `true`, roteia todas as requisições pelo YARP API Gateway |
| `TEST_EMAIL` | string | `loadtest@test.com` | Email do usuário de teste para autenticação JWT |
| `TEST_PASSWORD` | string | `LoadTest@123` | Senha do usuário de teste |
| `GATEWAY_URL` | string | `http://localhost:5006` | URL do YARP API Gateway |
| `USERS_URL` | string | `http://localhost:5001` | URL do Users Service |
| `TASKMANAGER_URL` | string | `http://localhost:5000` | URL do TaskManager Service |
| `FINANCIAL_URL` | string | `http://localhost:5003` | URL do Financial Service |
| `GYM_URL` | string | `http://localhost:5004` | URL do Gym Service |
| `NUTRITION_URL` | string | `http://localhost:5002` | URL do Nutrition Service |
| `NOTIFICATION_URL` | string | `http://localhost:5126` | URL do Notification Service |

---

## 11. Exportação e Visualização de Resultados

### 11.1 Console (padrão)

O k6 exibe um resumo detalhado no terminal ao finalizar:

```
          /\      |‾‾| /‾‾/   /‾‾/
     /\  /  \     |  |/  /   /  /
    /  \/    \    |     (   /   ‾‾\
   /          \   |  |\  \ |  (‾)  |
  / __________ \  |__| \__\ \_____/ .io

  scenarios: (100.00%) 1 scenario, 50 max VUs, 5m30s max duration
  default: Up to 50 looping VUs for 5m0s

     ✓ create-task: status 201
     ✓ get-all-tasks: status 200
     ✓ search-tasks: status 200
     ✓ update-task: status 2xx
     ✓ delete-task: status 2xx

     checks.....................: 100.00% ✓ 4520  ✗ 0
     http_req_duration..........: avg=89ms  min=12ms  med=72ms  max=1.2s  p(95)=210ms  p(99)=450ms
   ✓ http_req_failed............: 0.00%   ✓ 0      ✗ 4520
     http_reqs..................: 4520    15.07/s
```

### 11.2 JSON (detalhado)

```bash
k6 run --out json=results/output.json scenarios/taskmanager.test.js
```

### 11.3 CSV

```bash
k6 run --out csv=results/output.csv scenarios/taskmanager.test.js
```

### 11.4 Summary Export (JSON resumido)

```bash
k6 run --summary-export=results/summary.json scenarios/taskmanager.test.js
```

### 11.5 InfluxDB + Grafana (dashboard visual)

```bash
# Requer InfluxDB rodando
k6 run --out influxdb=http://localhost:8086/k6 scenarios/taskmanager.test.js
```

### 11.6 Prometheus (remote write)

```bash
k6 run --out experimental-prometheus-rw scenarios/taskmanager.test.js
```

### 11.7 Grafana Cloud k6

```bash
# Requer conta no Grafana Cloud
k6 cloud scenarios/taskmanager.test.js
```

### 11.8 Múltiplas saídas simultâneas

```bash
k6 run \
  --out json=results/output.json \
  --out csv=results/output.csv \
  scenarios/taskmanager.test.js
```

---

## 12. Mapa Completo de Endpoints Testados

### Total: 63 operações HTTP cobrindo 5 microserviços + gateway

| Microserviço | Endpoints Testados | Métodos HTTP |
|-------------|-------------------|--------------|
| Users | 3 | POST (register, login), GET (list users) |
| TaskManager | 9 | POST (create label, create task), GET (list labels, list tasks, search tasks, get by id), PUT (update task), DELETE (delete task, delete label) |
| Financial | 10 | POST (create category, create transaction), GET (list categories, search categories, list transactions, search transactions, get by id), PUT (update transaction), DELETE (delete transaction, delete category) |
| Gym | 11 | POST (create routine, create exercise, create routine-exercise, create session), GET (list routines, search routines, list exercises, list routine-exercises, list sessions), DELETE (session, routine-exercise, exercise, routine) |
| Nutrition | 14 | POST (create diary, create meal, create progress, create liquid-type, create liquid), GET (list diaries, list meals, meals by diary, list foods, search foods, list progresses, list liquid-types), DELETE (liquid, progress, meal, liquid-type, diary) |
| Gateway | 10 | POST (login, create task), GET (tasks, transactions, categories, routines, exercises, diaries, foods, users) |
| **TOTAL** | **63** | — |

---

## 13. Considerações e Boas Práticas

### Ordem Recomendada de Execução

1. **`smoke`** — Valide que todos os serviços estão respondendo antes de qualquer coisa
2. **`low`** — Estabeleça a baseline de performance
3. **`average`** — Valide o comportamento esperado sob carga normal
4. **`stress`** — Identifique os limites do sistema
5. **`spike`** — Teste a resiliência a picos
6. **`soak`** — Busque memory leaks e degradação (execute com monitoramento de infra)

### Monitoramento Complementar

Durante os testes de carga, monitore:
- **CPU e Memória** dos containers/processos
- **Conexões do PostgreSQL** (pool exhaustion)
- **Disco I/O** (especialmente durante soak tests)
- **Latência de rede** entre serviços
- **Logs de erro** dos microserviços

### Dados de Teste

- Cada iteração cria e deleta seus próprios dados (cleanup automático)
- Em caso de falha durante a execução, dados órfãos podem permanecer no banco
- Recomenda-se usar um banco de dados separado para testes de carga
- O cenário `users.test.js` cria novos usuários a cada iteração que **não são deletados** (limitação proposital para simular crescimento)

### Limitações Conhecidas

- Os testes dependem do Users Service para autenticação — se ele estiver fora, todos os cenários falham no setup
- O cleanup usa chamadas DELETE que contam nas métricas de performance
- O cenário de gateway não faz cleanup dos tasks criados via POST
- Enums são passados como inteiros ou strings conforme a API espera — verifique se o formato está correto para sua versão da API
