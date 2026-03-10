# LifeSync - Análise Estratégica, Técnica e Funcional Completa

> **Data:** 08/03/2026
> **Autor:** Arquiteto de Software Sênior
> **Escopo:** Diagnóstico completo de todos os microserviços do ecossistema LifeSync

---

## Sumário

1. [Visão Geral do Ecossistema](#visão-geral-do-ecossistema)
2. [TaskManager](#1-taskmanager)
3. [Financial](#2-financial)
4. [Nutrition](#3-nutrition)
5. [Gym](#4-gym)
6. [Users](#5-users)
7. [Notification](#6-notification)
8. [API Gateway (YARP)](#7-api-gateway-yarp)
9. [WebApp (Blazor)](#8-webapp-blazor)
10. [ClientApp (MAUI)](#9-clientapp-maui)
11. [BuildingBlocks & Core](#10-buildingblocks--core)
12. [Análise Consolidada](#análise-consolidada)
13. [Roadmap de Evolução](#roadmap-de-evolução)

---

## Visão Geral do Ecossistema

O **LifeSync** é uma plataforma de produtividade e bem-estar pessoal que integra gerenciamento de tarefas, controle financeiro, acompanhamento nutricional, treinos de academia e notificações em um ecossistema unificado de microserviços.

### Stack Tecnológico

| Componente | Tecnologia | Versão |
|---|---|---|
| Runtime | .NET | 10.0 |
| ORM | Entity Framework Core | 10.0.x |
| Banco de Dados | PostgreSQL | 18 |
| Mensageria | RabbitMQ | 7.2.0 (client) |
| API Gateway | YARP | 2.3.0 |
| Frontend Web | Blazor (Server + WASM) | 10.0 |
| Frontend Mobile | .NET MAUI | 9.0 |
| Containerização | Docker + Docker Compose | Latest |
| Proxy Reverso (Prod) | Traefik | Latest |

### Arquitetura Geral

```
┌──────────────────────────────────────────────┐
│           Camada de Apresentação             │
│  ┌──────────────┐    ┌───────────────────┐   │
│  │  MAUI (Mobile)│    │ Blazor (Web)      │   │
│  └──────┬───────┘    └────────┬──────────┘   │
└─────────┼─────────────────────┼──────────────┘
          │                     │
    ┌─────▼─────────────────────▼─────┐
    │     YARP API Gateway (6006)     │
    │  JWT Validation + CORS + Routing│
    └──────────────┬──────────────────┘
                   │
    ┌──────┬───────┼───────┬──────┬──────────┐
    ▼      ▼       ▼       ▼      ▼          ▼
 Users  TaskMgr  Finance  Nutri  Gym    Notification
    │      │       │       │      │          ▲
    └──────┴───────┴───────┴──────┘          │
           │               │                 │
    ┌──────▼──────┐  ┌─────▼─────┐    ┌──────┘
    │ PostgreSQL  │  │ RabbitMQ  │────┘
    └─────────────┘  └───────────┘
```

### Padrões Arquiteturais Compartilhados

- **Clean Architecture**: Domain → Application → Infrastructure → API
- **CQRS**: Commands e Queries com handlers dedicados
- **Result\<T\>**: Tratamento funcional de erros (sem exceções no fluxo de negócio)
- **Repository + Specification**: Abstração de acesso a dados com filtros composíveis
- **Domain Events**: Eventos de domínio para consistência eventual
- **Soft Delete**: `IsDeleted` em todas as entidades via `BaseEntity<T>`

---

## 1. TaskManager

### Visão Geral

Microserviço responsável pelo gerenciamento de tarefas pessoais com sistema de labels (etiquetas) coloridas. É o serviço **mais maduro** do ecossistema, com cobertura completa de testes e funcionalidades bem definidas.

### Funcionalidades Já Implementadas

**Gerenciamento de Tarefas (TaskItem):**
- CRUD completo (criar, ler, atualizar, deletar)
- Criação em lote (batch)
- Status: Pending → InProgress → Completed
- Prioridade: Low, Medium, High, Urgent
- Data de vencimento (DueDate) com validação de datas futuras
- Associação many-to-many com labels
- Filtros avançados: por status, prioridade, data, label, título, usuário
- Paginação e ordenação dinâmica
- Verificação de atraso (`IsOverdue()`)

**Gerenciamento de Labels (TaskLabel):**
- CRUD completo com criação em lote
- 9 cores disponíveis (Red, Green, Blue, Yellow, Purple, Orange, Pink, Brown, Gray)
- Conversão para código hexadecimal
- Filtros por cor, nome e usuário

**Background Service:**
- `DueDateReminderService`: Serviço em segundo plano que monitora tarefas próximas do vencimento
- Publica `TaskDueReminderEvent` via RabbitMQ para o Notification Service
- Intervalo e threshold configuráveis

**Testes:**
- **Unit Tests**: Handlers, validators, mappers, services (Moq + FluentAssertions + AutoFixture + Bogus)
- **Integration Tests**: Repositórios e serviços com Testcontainers.PostgreSql + Respawn
- **E2E Tests**: Endpoints completos com WebApplicationFactory + TestAuthHandler

### Avaliação Crítica

**Pontos Fortes:**
- Arquitetura exemplar — serve como referência para os demais microserviços
- Cobertura de testes em três níveis (unit, integration, e2e) é excelente e rara em projetos dessa escala
- O `DueDateReminderService` adiciona valor real ao usuário, demonstrando proatividade no design
- A relação many-to-many TaskItem ↔ TaskLabel é bem modelada, com validação de duplicatas no domínio
- Enums com extensões em português (`ToFriendlyString()`) mostram atenção à localização
- Separação clara entre DTOs de leitura (`TaskItemDTO`) e escrita (`CreateTaskItemDTO`)
- Error types bem definidos e granulares (`TaskItemErrors`, `TaskLabelErrors`)

**Pontos Fracos:**
- Não há conceito de **subtarefas** ou **checklists** dentro de uma tarefa
- Não existe **recorrência de tarefas** (tarefas que se repetem diariamente, semanalmente, etc.)
- Falta um **campo de notas/comentários** nas tarefas para contexto adicional
- Não há **ordenação manual** (drag-and-drop) das tarefas pelo usuário
- O `UserId` é passado como parâmetro nos endpoints, mas deveria ser extraído do JWT para segurança
- Falta **autorização por usuário** — qualquer usuário autenticado pode acessar tarefas de outro

### Melhorias no Backend

1. **Segurança - Extrair UserId do JWT**: Os handlers devem extrair `UserId` do `IHttpContextAccessor` ao invés de recebê-lo como parâmetro. Isso elimina a possibilidade de um usuário manipular dados de outros.

2. **Subtarefas**: Adicionar entidade `SubTaskItem` com relação pai-filho com `TaskItem`. Campos: Title, IsCompleted, Order. Permitiria checklists dentro de tarefas.

3. **Tarefas Recorrentes**: Adicionar campos `RecurrenceType` (None, Daily, Weekly, Monthly) e `RecurrenceEndDate` em `TaskItem`. Um background service criaria automaticamente novas tarefas com base na recorrência.

4. **Campo de Notas**: Adicionar `Notes` (texto longo) em `TaskItem` para contexto adicional e anotações.

5. **Ordenação Manual**: Adicionar campo `SortOrder` (int) para permitir drag-and-drop do frontend.

6. **Soft Delete nas Labels**: Atualmente `TaskLabel` não utiliza `IsDeleted` de forma consistente.

7. **Audit Log**: Implementar histórico de mudanças de status para análise de produtividade.

### Melhorias no Frontend

1. **Visualização Kanban**: Board com colunas por status (Pending | InProgress | Completed) com drag-and-drop.
2. **Calendário de Tarefas**: Visualização mensal/semanal das tarefas por data de vencimento.
3. **Indicador de Progresso**: Barra de progresso mostrando % de tarefas concluídas no dia/semana.
4. **Notificações In-App**: Toast notifications quando uma tarefa está próxima do vencimento.
5. **Filtros Rápidos**: Chips clicáveis para filtrar por prioridade/status sem abrir modal.
6. **Busca por Texto**: Campo de busca fulltext no título e descrição.
7. **Cores de Prioridade**: Indicadores visuais claros (bordas coloridas, ícones) por prioridade.

### Novas Funcionalidades Sugeridas

- **Templates de Tarefas**: Salvar conjuntos de tarefas como templates reutilizáveis
- **Compartilhamento**: Permitir compartilhar tarefas com outros usuários do sistema
- **Anexos**: Upload de arquivos/imagens associados a uma tarefa
- **Timer/Pomodoro**: Timer integrado para medir tempo gasto em tarefas
- **Gamificação**: Streak de dias consecutivos completando tarefas, badges por marcos

### Oportunidades de Integração

- **Nutrition**: Criar tarefas automáticas como "Registrar refeição" ou "Beber água" baseadas nas metas diárias
- **Gym**: Gerar tarefa "Treinar hoje" quando há sessão programada
- **Financial**: Criar tarefas de lembrete para contas a pagar baseadas em transações recorrentes
- **Dashboard Unificado**: Alimentar um painel central com métricas de produtividade

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Extrair UserId do JWT (segurança) |
| **Alta** | Autorização por usuário nos endpoints |
| **Alta** | Visualização Kanban no frontend |
| **Média** | Subtarefas/Checklists |
| **Média** | Tarefas recorrentes |
| **Média** | Calendário de tarefas |
| **Baixa** | Templates de tarefas |
| **Baixa** | Timer/Pomodoro integrado |
| **Baixa** | Gamificação |

### Impacto Esperado

As melhorias de segurança (JWT + autorização) são fundamentais para um ambiente multi-tenant. O Kanban board e o calendário transformariam a experiência do usuário, tornando o gerenciamento visual e intuitivo. Subtarefas e recorrência elevariam o serviço ao nível de ferramentas como Todoist e TickTick.

---

## 2. Financial

### Visão Geral

Microserviço responsável pelo controle financeiro pessoal, permitindo registro de receitas e despesas categorizadas, com suporte a múltiplas moedas e métodos de pagamento.

### Funcionalidades Já Implementadas

**Categorias:**
- CRUD completo
- Busca por nome (contains)
- Filtro por usuário
- Categorias por usuário (cada um tem as suas)

**Transações:**
- CRUD completo
- Tipos: Income (Receita), Expense (Despesa)
- Métodos de pagamento: Cash, CreditCard, DebitCard, BankTransfer, Pix, Other
- Value Object `Money`: Amount + Currency (140+ moedas suportadas com símbolos)
- Flag `IsRecurring` para transações recorrentes
- Datas em UTC
- Filtros avançados: por data (range), tipo, categoria, valor (igual, maior, menor), moeda, descrição
- Paginação e ordenação

**DTOs de Relatórios (Apenas definidos, não implementados):**
- `UserBalanceSummaryDTO`
- `AccountBalanceDTO`
- `ReportsController` (vazio)

**Testes:**
- Unit Tests: 38 testes de entidades (Category: 13, Transaction: 25)
- Integration Tests: Placeholder (não implementados)
- E2E Tests: Não existem

### Avaliação Crítica

**Pontos Fortes:**
- O Value Object `Money` com suporte a 140+ moedas é bem pensado e extensível
- O enum `PaymentMethod` inclui Pix, demonstrando atenção ao contexto brasileiro
- Filtros de transação são granulares (range de valores, range de datas)
- A flag `IsRecurring` antecipa uma funcionalidade importante
- Validações robustas nas entidades (amount != 0, data não no futuro)
- `TransactionType.ToFriendlyString()` retorna "Renda"/"Despesa" (localizado)

**Pontos Fracos:**
- **Relatórios não implementados**: O `ReportsController` está vazio. Para um app financeiro, relatórios são o core value. Sem eles, o serviço é apenas um CRUD glorificado.
- **Amount é `int`**: O campo `Money.Amount` é `int`, não `decimal`. Isso impede registrar centavos (R$ 19,90 por exemplo). **Erro crítico de modelagem.**
- **Transações recorrentes são apenas flag**: `IsRecurring = true` não gera automaticamente novas transações. É um dado sem processamento.
- **Sem conceito de Conta/Carteira**: Não há entidade `Account` ou `Wallet` para separar dinheiro por conta bancária, carteira digital, etc.
- **Sem orçamento (Budget)**: Não existe funcionalidade de definir orçamento mensal por categoria.
- **Sem anexos**: Não é possível anexar comprovantes ou fotos de recibos.
- **CategoryId nullable em Transaction**: Uma transação pode existir sem categoria, o que prejudica a organização dos dados.
- **Migração vazia**: `20250609025413_initialCreate321123` é uma migração vazia, poluindo o histórico.
- **Falta de testes de integração e E2E**: Apenas testes de entidade existem.

### Melhorias no Backend

1. **CRÍTICO - Corrigir tipo do Amount**: Mudar `Money.Amount` de `int` para `decimal`. Sem isso, o app é inutilizável para finanças reais.

2. **Implementar Relatórios**:
   - Saldo total por período
   - Gastos por categoria (top 5, gráfico de pizza)
   - Evolução de receitas vs despesas ao longo do tempo (gráfico de barras)
   - Média de gastos diários/mensais
   - Comparativo mês atual vs mês anterior

3. **Entidade Account/Wallet**:
   - Permitir múltiplas contas (Nubank, Itaú, Carteira, etc.)
   - Cada transação vinculada a uma conta
   - Saldo por conta

4. **Budget (Orçamento)**:
   - Entidade `Budget`: UserId, CategoryId, Month, Year, LimitAmount
   - Alertas quando o gasto se aproxima do limite
   - Porcentagem de uso do orçamento

5. **Processamento de Recorrência**:
   - Background service que gera transações automáticas baseadas em `IsRecurring`
   - Campos adicionais: `RecurrenceType` (Daily, Weekly, Monthly, Yearly), `RecurrenceEndDate`

6. **Categorias Padrão**: Seed de categorias comuns (Alimentação, Transporte, Moradia, Lazer, Saúde, Educação)

7. **Limpar Migrações**: Remover a migração vazia e consolidar o histórico

### Melhorias no Frontend

1. **Dashboard Financeiro**: Tela principal com cards de resumo (saldo, receitas, despesas do mês), gráficos de evolução
2. **Gráfico de Pizza por Categoria**: Visualização clara de onde o dinheiro está sendo gasto
3. **Gráfico de Barras Temporal**: Receitas vs despesas mês a mês
4. **Indicador de Orçamento**: Barras de progresso por categoria mostrando % do orçamento usado
5. **Tela de Contas/Carteiras**: Lista de contas com saldos individuais
6. **Registro Rápido**: Botão flutuante para registrar despesa rapidamente (valor + categoria)
7. **Filtros Visuais**: Período rápido (Hoje, Semana, Mês, Ano) com tabs
8. **Cores por Tipo**: Verde para receitas, vermelho para despesas, de forma consistente

### Novas Funcionalidades Sugeridas

- **Importação de Extrato**: Importar CSV/OFX de bancos para registrar transações automaticamente
- **Metas Financeiras**: Definir metas de economia (ex: "Juntar R$ 5.000 até Dezembro")
- **Split de Despesas**: Dividir despesas com outros usuários
- **Projeção de Gastos**: Com base no histórico, projetar gastos para o restante do mês
- **Insights com IA**: "Você gastou 30% mais em alimentação esse mês comparado à média"

### Oportunidades de Integração

- **TaskManager**: Criar tarefas automáticas de "Pagar conta" para transações recorrentes
- **Notification**: Alertas quando o orçamento de uma categoria está no limite
- **Dashboard**: Alimentar painel unificado com saldo e resumo financeiro

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Corrigir Money.Amount de int para decimal |
| **Alta** | Implementar relatórios básicos |
| **Alta** | Dashboard financeiro no frontend |
| **Média** | Entidade Account/Wallet |
| **Média** | Budget (Orçamento) por categoria |
| **Média** | Processamento de transações recorrentes |
| **Média** | Gráficos de pizza e barras |
| **Baixa** | Importação de extratos bancários |
| **Baixa** | Metas financeiras |
| **Baixa** | Insights com IA |

### Impacto Esperado

A correção do tipo `Money.Amount` é bloqueante — sem ela, o app não serve para uso real. Os relatórios são o valor central de um app financeiro: sem eles, o usuário não tem motivo para continuar registrando transações. O dashboard com gráficos transformaria a experiência de "planilha glorificada" para "ferramenta financeira inteligente".

---

## 3. Nutrition

### Visão Geral

Microserviço responsável pelo acompanhamento nutricional diário, permitindo registrar refeições, alimentos consumidos e ingestão de líquidos, com cálculo automático de calorias e acompanhamento de metas diárias.

### Funcionalidades Já Implementadas

**DailyProgress (Progresso Diário):**
- Rastreamento de calorias consumidas e líquidos ingeridos por dia
- Value Object `DailyGoal` com metas de calorias e líquidos
- Cálculo de porcentagem de progresso (`GetCaloriesProgressPercentage()`, `GetLiquidsProgressPercentage()`)
- Verificação de metas atingidas (`IsGoalMet()`)
- Propagação automática de metas para datas futuras (`SetGoal`)
- Filtros por usuário e data

**Diary (Diário Alimentar):**
- CRUD completo
- Associação com Meals e Liquids
- Cálculo de calorias totais (soma de todas as refeições)
- Total de líquidos por diário
- Filtro por usuário e data

**Meal (Refeição):**
- CRUD completo
- Associação com MealFoods (alimentos da refeição)
- Cálculo de calorias totais (soma dos MealFoods)
- Comandos dedicados: `AddMealFood`, `RemoveMealFood`

**MealFood (Alimento na Refeição):**
- Relação entre Meal e Food com quantidade em gramas
- Cálculo proporcional de calorias: `Quantity * Food.Calories / 100`
- CRUD completo

**Food (Alimentos - Tabela de Referência):**
- Base de dados nutricional populada via CSV (seed)
- Campos: Nome, Calorias, Proteínas, Lipídeos, Carboidratos, Cálcio, Magnésio, Ferro, Sódio, Potássio
- Somente leitura (sem CRUD de escrita na API)

**Liquid (Líquidos):**
- Registro de líquidos consumidos com quantidade em ML
- Associação com `LiquidType`
- CRUD completo

**LiquidType (Tipos de Líquido):**
- Tipos cadastráveis (Água, Suco, Café, etc.)
- CRUD completo

**Domain Events (Sincronização Automática):**
- `MealFoodAddedEvent` → Atualiza calorias no DailyProgress
- `MealFoodRemovedEvent` → Decrementa calorias no DailyProgress
- `LiquidChangedEvent` → Atualiza líquidos no DailyProgress
- `MealAddedToDiaryEvent` → Evento quando refeição é adicionada

**Testes:** Todos os três projetos de teste estão vazios (placeholders).

### Avaliação Crítica

**Pontos Fortes:**
- A sincronização automática via domain events (MealFood → DailyProgress) é **excelente**. O usuário registra comida e o progresso se atualiza sozinho, sem intervenção.
- A propagação de metas para datas futuras é muito prática — define uma vez e vale para os próximos dias.
- O seed de alimentos via CSV permite popular rapidamente uma base nutricional realista.
- Os cálculos proporcionais de nutrientes (`Quantity * Calories / 100`) são corretos e padronizados (base 100g).
- O Value Object `DailyGoal` encapsula bem a lógica de metas.
- 7 entidades bem definidas com responsabilidades claras.

**Pontos Fracos:**
- **Sem macronutrientes no DailyProgress**: Só rastreia calorias e líquidos. Não acompanha proteínas, carboidratos e gorduras, que são essenciais para qualquer dieta.
- **Sem tipo de refeição**: `Meal` não tem enum para tipo (Café da manhã, Almoço, Lanche, Jantar, Ceia). Dificulta organização.
- **Food é somente leitura**: O usuário não pode cadastrar alimentos personalizados. Se comeu algo que não está na base CSV, não pode registrar.
- **Sem favoritos/histórico**: Não há forma rápida de re-registrar refeições frequentes.
- **Testes completamente ausentes**: Para um serviço com lógica de domínio complexa (eventos, cálculos), a falta de testes é um risco considerável.
- **Data do DailyProgress não pode ser no passado**: A validação `date < DateOnly.FromDateTime(DateTime.UtcNow)` impede registrar dados retroativos, o que é impraticável — o usuário pode esquecer de registrar uma refeição ontem.
- **Sem cálculo de água ideal**: Não sugere quantidade ideal de água baseada em peso/atividade.
- **Sem integração com Gym**: Não considera calorias gastas no treino.

### Melhorias no Backend

1. **Rastrear Macronutrientes**: Adicionar `TotalProtein`, `TotalCarbs`, `TotalFat` em `DailyProgress` e `DailyGoal`. Calcular automaticamente via domain events, similar às calorias.

2. **Enum MealType**: Adicionar `MealType` (Breakfast, Lunch, Snack, Dinner, Supper) em `Meal` para categorização.

3. **Alimentos Personalizados**: Permitir que o usuário cadastre alimentos que não existem na base. Adicionar `IsCustom` e `UserId` em `Food`.

4. **Permitir Datas Passadas**: Remover a validação que impede registrar em datas passadas. Limitar apenas a datas razoáveis (ex: últimos 30 dias).

5. **Receitas Favoritas**: Nova entidade `FavoriteMeal` que salva combinações de MealFoods para registro rápido.

6. **Histórico de Nutrição**: Endpoint para retornar evolução nutricional ao longo do tempo (semana, mês).

7. **Implementar Testes**: Priorizar unit tests para domain events e cálculos de calorias/macros.

### Melhorias no Frontend

1. **Tela de Progresso Diário**: Gráficos circulares mostrando % de calorias, proteínas, carboidratos e gorduras em relação à meta.
2. **Registro Rápido**: Botão "Repetir refeição de ontem" ou favoritos para registro em 2 toques.
3. **Busca de Alimentos**: Autocomplete com a base de Foods, mostrando calorias ao lado do nome.
4. **Timeline do Dia**: Visualização cronológica das refeições e líquidos do dia.
5. **Gráfico Semanal**: Evolução de calorias consumidas na semana com linha de meta.
6. **Scanner de Código de Barras**: (Mobile) Escanear código de barras de alimentos industrializados.
7. **Indicadores Visuais de Hidratação**: Copinhos d'água preenchidos para visualizar progresso de líquidos.

### Novas Funcionalidades Sugeridas

- **Planos Alimentares**: Templates de dieta semanal que o usuário pode seguir
- **Sugestão de Refeição**: Com base nas metas restantes do dia, sugerir alimentos para completar macros
- **Integração com Gym**: Calorias gastas no treino subtraídas do balanço calórico diário
- **Foto da Refeição**: Upload de foto para registro visual do que foi comido
- **Streak de Hidratação**: Gamificação — dias consecutivos atingindo meta de água

### Oportunidades de Integração

- **Gym**: Sincronizar calorias gastas no treino para cálculo de balanço calórico líquido
- **TaskManager**: Gerar tarefas automáticas ("Registrar almoço", "Beber água") baseadas nas metas
- **Notification**: Lembretes para registrar refeições e beber água em intervalos configuráveis
- **Dashboard**: Resumo nutricional do dia no painel unificado

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Rastrear macronutrientes (proteína, carbs, gordura) |
| **Alta** | Permitir registro em datas passadas |
| **Alta** | Implementar testes unitários |
| **Alta** | Enum MealType (tipo de refeição) |
| **Média** | Alimentos personalizados |
| **Média** | Tela de progresso com gráficos |
| **Média** | Registro rápido / favoritos |
| **Baixa** | Planos alimentares |
| **Baixa** | Scanner de código de barras |
| **Baixa** | Integração com Gym |

### Impacto Esperado

O rastreamento de macronutrientes é essencial para qualquer pessoa que faz dieta seriamente — sem ele, o app só atende quem conta calorias de forma superficial. O registro em datas passadas é questão de usabilidade básica. A tela de progresso com gráficos circulares tornaria a experiência visual e motivadora, similar ao MyFitnessPal.

---

## 4. Gym

### Visão Geral

Microserviço responsável pelo gerenciamento de treinos de academia, incluindo cadastro de exercícios, montagem de rotinas, registro de sessões de treino e acompanhamento de exercícios completados.

### Funcionalidades Já Implementadas

**Exercise (Exercícios):**
- CRUD completo
- Enum `MuscleGroup`: 16 grupos musculares (Chest, Back, Shoulders, Biceps, etc.)
- Enum `ExerciseType`: 8 tipos (Strength, Hypertrophy, Endurance, Power, Flexibility, Cardio, HIIT, Recovery)
- Enum `EquipmentType`: 13 tipos (Barbell, Dumbbell, Machine, Cable, Bodyweight, etc.)
- Filtros avançados por grupo muscular, tipo, equipamento

**Routine (Rotinas de Treino):**
- CRUD completo
- Coleção de `RoutineExercise` com configurações específicas
- Adição/remoção de exercícios

**RoutineExercise (Exercício na Rotina):**
- Value Objects: `SetCount`, `RepetitionCount`, `RestTime`, `Weight`
- Instruções opcionais
- Peso recomendado (com unidade de medida)

**TrainingSession (Sessão de Treino):**
- CRUD com rastreamento de início/fim
- Vinculação com rotina
- Coleção de exercícios completados
- Cálculo de duração (`GetDuration()`)
- Notas opcionais

**CompletedExercise (Exercício Completado):**
- Séries e repetições realmente feitas
- Peso utilizado
- Timestamp de conclusão
- Notas opcionais

**Value Objects:**
- `Weight`: valor + unidade (kg/lb)
- `SetCount`, `RepetitionCount`: inteiros positivos validados
- `RestTime`: segundos de descanso
- `Duration`: TimeSpan de duração

**Enums adicionais:**
- `ActivityLevel`: Beginner, Intermediate, Advanced, Expert
- `MeasurementUnit`: Kilogram, Pound, Meter, Kilometer, etc.

**Testes:** Todos os três projetos de teste estão vazios (placeholders).

### Avaliação Crítica

**Pontos Fortes:**
- Modelo de domínio **muito bem estruturado**. A separação entre Exercise → Routine → RoutineExercise → TrainingSession → CompletedExercise reflete perfeitamente o fluxo real de treino.
- Os Value Objects para séries, repetições, descanso e peso adicionam type safety e validação no nível mais baixo.
- 5 entidades com 5 controladores dedicados demonstram boa granularidade.
- Suporte a múltiplas unidades de medida (kg/lb) é importante para internacionalização.
- Os enums de MuscleGroup (16 valores) e EquipmentType (13 valores) são abrangentes.

**Pontos Fracos:**
- **Sem histórico de progressão**: Não há funcionalidade para visualizar evolução de peso/repetições ao longo do tempo em um exercício específico. Este é o principal valor de um app de academia.
- **Sem exercícios pré-cadastrados (seed)**: O usuário precisa cadastrar manualmente todos os exercícios. Nenhum app de academia funciona assim — deveria ter uma base de exercícios populares.
- **Sem cálculo de volume de treino**: Métricas como volume total (séries × repetições × peso) não são calculadas.
- **Sem templates de rotina**: Não há rotinas prontas (Push/Pull/Legs, Upper/Lower, Full Body) para o usuário usar como base.
- **Sem planejamento semanal**: Não há conceito de "plano de treino semanal" com dias definidos.
- **Domain Events comentados**: `TrainingSessionCompletedEvent` está referenciado mas não implementado.
- **Testes completamente ausentes**: Para 5 entidades com value objects e lógica de domínio, a ausência de testes é preocupante.
- **Sem cálculo de calorias gastas**: Não estima calorias queimadas por sessão de treino.

### Melhorias no Backend

1. **Seed de Exercícios**: Popular base com 50-100 exercícios comuns (Supino, Agachamento, Rosca, etc.) com grupo muscular e equipamento pré-definidos.

2. **Histórico de Progressão**: Endpoint para retornar evolução de peso/repetições por exercício ao longo do tempo. Query: `GET /api/exercises/{id}/progression?userId={userId}&period=3months`

3. **Métricas de Volume**: Calcular volume total por sessão (séries × reps × peso) e por grupo muscular.

4. **Personal Records (PRs)**: Rastrear e retornar recordes pessoais por exercício (maior peso, mais repetições, maior volume).

5. **Plano Semanal**: Nova entidade `WeeklyPlan` com slots por dia da semana, cada um associado a uma rotina.

6. **Templates de Rotina**: Rotinas pré-montadas (Push/Pull/Legs, PPL, Bro Split, Full Body) como seed.

7. **Estimativa de Calorias**: Calcular calorias gastas por sessão com base na duração e tipo de exercício.

8. **Implementar TrainingSessionCompletedEvent**: Publicar evento ao concluir sessão para integração com Nutrition.

### Melhorias no Frontend

1. **Tela de Treino Ativo**: Interface durante o treino mostrando exercício atual, séries restantes, timer de descanso, botão de próximo.
2. **Timer de Descanso**: Contagem regressiva automática entre séries com alerta sonoro.
3. **Gráficos de Evolução**: Linha de progressão de peso por exercício ao longo das semanas.
4. **Calendário de Treinos**: Visualização mensal com dias treinados destacados.
5. **Resumo Pós-Treino**: Tela ao finalizar sessão mostrando duração, volume total, PRs batidos.
6. **Biblioteca de Exercícios**: Galeria com exercícios organizados por grupo muscular com imagem/GIF demonstrativo.
7. **Comparativo de Sessões**: Comparar performance da sessão atual com a anterior.

### Novas Funcionalidades Sugeridas

- **Vídeos de Exercícios**: Link para vídeo demonstrativo de cada exercício
- **Supersets e Dropsets**: Suporte a técnicas avançadas de treino
- **Body Measurements**: Rastreamento de medidas corporais (peso, circunferências)
- **1RM Calculator**: Calculadora de repetição máxima estimada
- **Rest Day Recommendation**: Sugestão de descanso baseada em frequência e volume

### Oportunidades de Integração

- **Nutrition**: Publicar `TrainingSessionCompletedEvent` com calorias gastas para ajustar balanço calórico
- **TaskManager**: Gerar tarefa "Treinar hoje" quando há sessão planejada
- **Notification**: Lembretes de treino e parabenização por PRs
- **Dashboard**: Dias treinados no mês, streak de treinos, volume semanal

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Seed de exercícios populares |
| **Alta** | Histórico de progressão de peso |
| **Alta** | Timer de descanso no frontend |
| **Alta** | Implementar testes |
| **Média** | Métricas de volume e PRs |
| **Média** | Plano semanal de treino |
| **Média** | Tela de treino ativo |
| **Média** | Gráficos de evolução |
| **Baixa** | Body measurements |
| **Baixa** | 1RM Calculator |
| **Baixa** | Estimativa de calorias |

### Impacto Esperado

O seed de exercícios elimina a maior barreira de adoção — cadastrar exercícios manualmente é tedioso e frustrante. O histórico de progressão é literalmente o motivo pelo qual pessoas usam apps de academia (Strong, Hevy, JEFIT). O timer de descanso é funcionalidade básica esperada. Juntas, essas melhorias transformariam o serviço de um cadastro de treinos para uma ferramenta real de acompanhamento.

---

## 5. Users

### Visão Geral

Microserviço responsável pela autenticação, autorização e gerenciamento de perfis de usuário. Centraliza o fluxo de identidade para todo o ecossistema.

### Funcionalidades Já Implementadas

**Autenticação:**
- Login com email e senha (ASP.NET Identity)
- Registro de novo usuário
- Logout
- JWT Access Token (HS256, 60 min de expiração)
- Refresh Token (64 bytes, 7 dias)
- Recuperação de senha (Forgot Password / Reset Password)
- Alteração de senha (Change Password)
- Confirmação de email

**Gerenciamento de Usuário:**
- Atualização de perfil
- Consulta por ID
- Listagem de todos os usuários
- Soft delete / desativação

**Value Objects:**
- `Name`: FirstName, LastName, FullName (calculado)
- `Contact`: Email com validação via regex

**Domain Events:**
- `UserRegisteredEvent`: Publicado via RabbitMQ ao registrar novo usuário

**Configurações de Segurança:**
- Senha: mínimo 6 caracteres, lowercase obrigatório
- Lockout: 5 tentativas falhas → 5 minutos de bloqueio
- Email único obrigatório

### Avaliação Crítica

**Pontos Fortes:**
- A integração com ASP.NET Identity é sólida — delegar autenticação para o framework é a decisão correta
- O Refresh Token flow está implementado corretamente
- O evento `UserRegisteredEvent` via RabbitMQ demonstra boa integração com o Notification Service
- Value Objects `Name` e `Contact` encapsulam validações de domínio
- Account lockout protege contra ataques de força bruta

**Pontos Fracos:**
- **Chave JWT hardcoded no appsettings**: `SuperSecretKeyForJWTAuthentication2024!@#$%` está exposta nos arquivos de configuração. Deveria estar em variáveis de ambiente ou secret manager.
- **Sem OAuth/Social Login**: Não suporta login com Google, Apple, Microsoft — funcionalidade esperada em apps modernos.
- **Senha fraca**: Mínimo 6 caracteres sem exigir maiúsculas, números ou caracteres especiais é insuficiente para segurança moderna.
- **Sem 2FA**: Two-Factor Authentication não está implementado.
- **GET /users retorna todos os usuários**: Sem autorização por role — qualquer usuário autenticado pode listar todos os outros.
- **Email confirmation não obrigatório**: `RequireConfirmedEmail` pode estar desabilitado.
- **Sem rate limiting**: Endpoints de login e registro vulneráveis a ataques de força bruta (além do lockout).
- **Sem avatar/foto de perfil**: Funcionalidade básica de perfil ausente.
- **BirthDate sem validação de idade mínima**: Aceita qualquer data.
- **`SecureSocketOptions.None` no SMTP**: Email sem TLS é inseguro em produção.

### Melhorias no Backend

1. **Mover JWT Key para variáveis de ambiente**: Usar `IConfiguration` com User Secrets (dev) e Environment Variables (prod).

2. **OAuth/Social Login**: Implementar login com Google (prioridade) e Apple. ASP.NET Identity suporta nativamente.

3. **Fortalecer Política de Senha**: Exigir 8+ caracteres, 1 maiúscula, 1 número, 1 caractere especial.

4. **Rate Limiting**: Implementar `AspNetCoreRateLimit` ou middleware nativo do .NET nos endpoints de autenticação.

5. **2FA (Two-Factor Authentication)**: Suportar TOTP (Google Authenticator) usando ASP.NET Identity.

6. **Autorização por Role**: Implementar roles (Admin, User) e proteger endpoints administrativos (GET /users).

7. **Avatar/Foto de Perfil**: Campo de URL para foto de perfil, com upload para blob storage.

8. **Validação de Idade**: Mínimo 13 anos (LGPD/COPPA compliance).

9. **SMTP com TLS**: Configurar `SecureSocketOptions.StartTls` para produção.

### Melhorias no Frontend

1. **Tela de Perfil**: Foto, nome, email, data de nascimento editáveis com preview.
2. **Onboarding Flow**: Wizard de boas-vindas pós-registro com configuração de metas e preferências.
3. **Login Social**: Botões de "Login com Google" e "Login com Apple" na tela de login.
4. **Indicador de Força de Senha**: Barra visual mostrando força da senha durante o cadastro.
5. **Toggle de Tema**: Modo escuro/claro no perfil.

### Novas Funcionalidades Sugeridas

- **Perfil de Saúde**: Peso, altura, IMC, nível de atividade — alimentar outros microserviços
- **Preferências do App**: Notificações, idioma, unidade de medida (kg/lb, km/mi)
- **Exportação de Dados**: LGPD compliance — permitir download de todos os dados do usuário
- **Deleção de Conta**: LGPD compliance — permitir deleção completa da conta

### Oportunidades de Integração

- **Todos os Serviços**: UserId centralizado é fundamental — garantir que todos os serviços validem o token consistentemente
- **Notification**: Preferências de notificação por tipo (email, push, in-app)
- **Gym/Nutrition**: Perfil de saúde (peso, altura) alimenta cálculos nutricionais e de treino

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Mover JWT Key para env vars (segurança) |
| **Alta** | Fortalecer política de senha |
| **Alta** | Rate limiting nos endpoints de auth |
| **Alta** | Autorização por role |
| **Média** | OAuth/Social Login (Google) |
| **Média** | Avatar/foto de perfil |
| **Média** | SMTP com TLS |
| **Baixa** | 2FA |
| **Baixa** | Exportação/Deleção LGPD |

### Impacto Esperado

As melhorias de segurança (JWT key, senhas, rate limiting) são fundamentais para proteger dados pessoais sensíveis. Login social reduz dramaticamente a fricção de cadastro — estudos mostram que 50%+ dos usuários preferem social login. O onboarding wizard aumenta retenção nos primeiros dias.

---

## 6. Notification

### Visão Geral

Microserviço responsável pelo envio de notificações, atualmente limitado a emails transacionais. Opera de forma 100% event-driven, sem endpoints REST, consumindo eventos do RabbitMQ.

### Funcionalidades Já Implementadas

**Consumers de Eventos:**
- `UserRegisteredIntegrationEvent` (exchange: `user_exchange`, routing key: `user.registered`)
- `TaskDueReminderIntegrationEvent` (exchange: `task_exchange`, routing key: `task.due.reminder`)

**Strategy Pattern para Emails:**
- `UserRegisteredEmailStrategy`: Template de email de boas-vindas
- `OrderPlacedEmailStrategy`: Placeholder para futura funcionalidade
- `EmailEventStrategyResolver`: Resolve strategy por tipo de evento

**Envio de Email:**
- SMTP via MailKit
- Suporte a host, porta, credenciais configuráveis
- MailHog em desenvolvimento, SendGrid em produção

**Persistência:**
- Salva `EmailMessage` no banco para auditoria

### Avaliação Crítica

**Pontos Fortes:**
- O design event-driven é correto para um serviço de notificações — desacoplamento total
- O Strategy Pattern permite adicionar novos tipos de email facilmente
- A persistência para auditoria é uma boa prática
- A configuração de RabbitMQ com durable queues garante confiabilidade

**Pontos Fracos:**
- **Templates de email hardcoded**: "Welcome!" e "Thanks for registering." são primitivos. Sem HTML, sem personalização.
- **Sem templates HTML**: Emails são texto puro, sem design, sem branding.
- **Apenas email**: Não suporta push notifications (mobile), in-app notifications, ou SMS.
- **Sem retry/dead letter**: Se o envio falhar, o email é perdido. Não há retry automático ou dead letter queue.
- **`SecureSocketOptions.None`**: SMTP sem TLS em produção é inseguro.
- **Sem fila de envio**: Emails são enviados sincronamente dentro do event handler. Se o SMTP estiver lento, pode acumular mensagens.
- **Sem preferências de notificação**: O usuário não pode configurar quais notificações deseja receber.
- **`OrderPlacedEmailStrategy` sem uso**: Placeholder sem integração real.
- **Sem status de entrega**: Não rastreia se o email foi entregue, aberto ou bounced.

### Melhorias no Backend

1. **Templates HTML**: Implementar templates com Razor ou Scriban para emails com layout profissional, logo, botões.

2. **Push Notifications**: Integrar Firebase Cloud Messaging (FCM) para notificações mobile no app MAUI.

3. **In-App Notifications**: Nova entidade `Notification` com status (Read/Unread), endpoints REST para o frontend consumir.

4. **Retry com Dead Letter Queue**: Implementar política de retry (3 tentativas com backoff) e dead letter queue para falhas permanentes.

5. **Fila de Envio Assíncrona**: Usar um `Channel<T>` ou fila interna para desacoplar recebimento do evento do envio real do email.

6. **Preferências de Notificação**: Endpoint para o usuário configurar quais tipos de notificação deseja receber e por qual canal (email, push, in-app).

7. **Status de Entrega**: Rastrear status (Queued, Sent, Delivered, Bounced, Failed) em `EmailMessage`.

8. **SMTP com TLS**: Configurar `SecureSocketOptions.StartTls` para segurança em produção.

### Melhorias no Frontend

1. **Central de Notificações**: Ícone de sino com badge de contagem de não lidas.
2. **Lista de Notificações**: Dropdown ou tela com histórico de notificações in-app.
3. **Configurações de Notificação**: Tela para toggle de cada tipo de notificação por canal.
4. **Notificações Push**: Notificações nativas no mobile (MAUI) com deep linking para a tela relevante.

### Novas Funcionalidades Sugeridas

- **Notificações Agendadas**: Permitir agendar lembretes personalizados
- **Templates por Idioma**: Suporte a múltiplos idiomas nos templates de email
- **Webhooks**: Notificar sistemas externos quando eventos ocorrem
- **Digest Email**: Resumo diário/semanal com progresso em todos os módulos

### Oportunidades de Integração

- **Financial**: Alertas de orçamento atingido ou transação recorrente próxima
- **Nutrition**: Lembretes para registrar refeições e beber água
- **Gym**: Notificação de dia de treino e parabenização por PRs
- **TaskManager**: Já integrado — melhorar template do lembrete de vencimento

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Templates HTML para emails |
| **Alta** | Retry/Dead Letter Queue |
| **Alta** | In-App Notifications |
| **Média** | Push Notifications (FCM) |
| **Média** | Preferências de notificação do usuário |
| **Média** | SMTP com TLS |
| **Baixa** | Digest email semanal |
| **Baixa** | Webhooks |
| **Baixa** | Notificações agendadas |

### Impacto Esperado

Templates HTML profissionais melhoram a percepção de qualidade do produto. In-app notifications mantêm o usuário engajado sem sair do app. Push notifications são essenciais para mobile — sem elas, o app é silencioso. O retry/DLQ garante que nenhuma notificação importante seja perdida.

---

## 7. API Gateway (YARP)

### Visão Geral

Gateway centralizado utilizando YARP (Yet Another Reverse Proxy) da Microsoft, responsável por roteamento, validação JWT e CORS para todo o ecossistema.

### Funcionalidades Já Implementadas

- Proxy reverso com YARP 2.3.0
- Validação JWT centralizada
- CORS configurado para Blazor app (localhost:6007)
- Data Protection Keys persistidos em volume `/keys`
- Roteamento por prefixo de serviço:
  - `/auth/*` → Users API
  - `/taskmanager-service/*` → TaskManager API
  - `/nutrition-service/*` → Nutrition API
  - `/financial-service/*` → Financial API
  - `/users-service/*` → Users API
  - `/gym-service/*` → Gym API

### Avaliação Crítica

**Pontos Fortes:**
- YARP é escolha excelente — nativo do ecossistema .NET, performático e bem mantido pela Microsoft
- Validação JWT centralizada elimina duplicação de configuração nos serviços
- Data Protection Keys em volume garante persistência entre restarts
- Configuração via appsettings facilita manutenção

**Pontos Fracos:**
- **Inconsistência de autorização**: Apenas o Gym service tem `RequireAuthentication` no gateway. Os demais delegam para os serviços internos, criando inconsistência.
- **Sem rate limiting global**: Vulnerável a ataques DDoS e abuso de API.
- **Sem cache de respostas**: Queries frequentes não são cacheadas.
- **Sem health check aggregado**: Não verifica saúde dos serviços downstream.
- **Sem circuit breaker**: Se um serviço ficar indisponível, as requisições continuam sendo enviadas.
- **Single destination por cluster**: Sem load balancing real.
- **CORS restrito a localhost**: Precisa ser configurado para domínio de produção.

### Melhorias no Backend

1. **Padronizar Autorização**: Todas as rotas (exceto `/auth/login` e `/auth/register`) devem ter `RequireAuthentication`.
2. **Rate Limiting**: Implementar rate limiting global por IP e por usuário usando `Microsoft.AspNetCore.RateLimiting`.
3. **Health Check Aggregado**: Endpoint `/health` que verifica saúde de todos os serviços downstream.
4. **Response Caching**: Cache de respostas para queries GET com TTL curto (30s-60s).
5. **Circuit Breaker**: Integrar Polly para circuit breaker nos clusters, evitando cascata de falhas.
6. **Logging Centralizado**: Request/response logging com correlation ID para rastreamento.
7. **API Versioning**: Suporte a versionamento de API no gateway.

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Padronizar autorização em todas as rotas |
| **Alta** | Rate limiting |
| **Média** | Health check aggregado |
| **Média** | Circuit breaker |
| **Média** | Logging com correlation ID |
| **Baixa** | Response caching |
| **Baixa** | API versioning |

### Impacto Esperado

A padronização de autorização fecha brechas de segurança. Rate limiting protege contra abusos. Health checks e circuit breaker aumentam a resiliência e observabilidade do sistema.

---

## 8. WebApp (Blazor)

### Visão Geral

Aplicação web construída com Blazor Hybrid (Server + WebAssembly), servindo como frontend web do LifeSync.

### Funcionalidades Já Implementadas

- Autenticação via JWT com `CustomAuthStateProvider`
- Token armazenado em localStorage
- `ApiClient` genérico com injeção automática de Bearer token
- Services para todos os microserviços (Financial, Gym, Nutrition, TaskManager)
- Componentes Syncfusion Blazor (licenciado)
- Razor Components com render modes interativos

### Avaliação Crítica

**Pontos Fortes:**
- O `ApiClient` genérico é bem implementado — abstrai toda comunicação HTTP com tipagem forte
- `CustomAuthStateProvider` lida corretamente com expiração de token
- A arquitetura de Services por domínio espelha os microserviços, facilitando manutenção

**Pontos Fracos:**
- **JWT em localStorage**: Vulnerável a ataques XSS. O ideal seria HttpOnly cookies.
- **Sem tratamento global de erros HTTP**: 401 não redireciona para login automaticamente.
- **Sem refresh automático de token**: Quando o access token expira, o usuário é deslogado abruptamente.
- **Sem loading states globais**: Falta indicador de carregamento durante chamadas API.
- **Sem offline support**: Nenhuma funcionalidade offline.
- **Sem PWA**: Não está configurado como Progressive Web App.

### Melhorias no Frontend

1. **Token em HttpOnly Cookie**: Migrar armazenamento do JWT de localStorage para HttpOnly secure cookie (via BFF pattern).
2. **Interceptor de 401**: Redirecionar para login automaticamente em respostas 401.
3. **Refresh Token Automático**: Interceptar erros 401, usar refresh token para obter novo access token transparentemente.
4. **Loading States**: Componente global de loading durante chamadas API.
5. **PWA**: Configurar como Progressive Web App com service worker para funcionalidades offline básicas.
6. **Dark Mode**: Tema escuro com toggle no perfil.
7. **Responsividade**: Garantir que todas as telas funcionem em mobile web.

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Token em HttpOnly Cookie (segurança) |
| **Alta** | Refresh token automático |
| **Alta** | Interceptor de 401 |
| **Média** | Loading states globais |
| **Média** | Dark mode |
| **Baixa** | PWA |
| **Baixa** | Offline support |

---

## 9. ClientApp (MAUI)

### Visão Geral

Aplicação mobile multiplataforma construída com .NET MAUI, suportando Android, iOS, macOS e Windows.

### Funcionalidades Já Implementadas

- Autenticação com JWT via `AuthDelegatingHandler`
- Navegação via Shell com rotas para todas as features
- Services para todos os microserviços
- Telas para: Auth, TaskManager, Financial, Nutrition
- Converters XAML customizados (cores de status, prioridade, etc.)
- Componentes customizados: `CircularProgressView`, `CustomTabBar`
- Modals para criação/edição de entidades
- Filtros e busca

### Avaliação Crítica

**Pontos Fortes:**
- Cobertura de features abrangente — telas para TaskManager, Financial e Nutrition
- Os converters XAML são bem pensados, trazendo consistência visual
- `CircularProgressView` customizado para progresso nutricional
- `AuthDelegatingHandler` injeta token automaticamente em todas as requisições

**Pontos Fracos:**
- **Telas de Gym ausentes**: O serviço mais rico em funcionalidades não tem telas mobile.
- **Sem notificações push**: O app é silencioso — não avisa sobre tarefas, treinos ou metas.
- **Sem biometria**: Não suporta login com fingerprint/Face ID.
- **Sem cache/offline**: Todas as operações dependem de internet.
- **Target framework net9.0**: Enquanto o backend usa .NET 10, o MAUI está em 9.0 — potencial incompatibilidade.

### Melhorias no Frontend

1. **Telas de Gym**: Implementar todas as telas de exercícios, rotinas, sessões de treino.
2. **Push Notifications**: Integrar FCM/APNs para lembretes e alertas.
3. **Biometria**: Login com fingerprint/Face ID usando SecureStorage.
4. **Cache Offline**: Cache de dados com SQLite local para funcionar sem internet.
5. **Dashboard Unificado**: Tela principal com resumo de todos os módulos.
6. **Animações e Microinterações**: Adicionar transições e feedback tátil.
7. **Atualizar para .NET 10**: Manter alinhado com o backend.

### Prioridade das Melhorias

| Prioridade | Melhoria |
|---|---|
| **Alta** | Telas de Gym |
| **Alta** | Dashboard unificado |
| **Média** | Push notifications |
| **Média** | Biometria |
| **Média** | Atualizar para .NET 10 |
| **Baixa** | Cache offline |
| **Baixa** | Animações |

---

## 10. BuildingBlocks & Core

### Visão Geral

Bibliotecas compartilhadas que fornecem a fundação arquitetural para todos os microserviços.

### Funcionalidades Já Implementadas

**BuildingBlocks:**
- CQRS: `ICommand<T>`, `IQuery<T>`, `ICommandHandler`, `IQueryHandler`, `ISender`, `IPublisher`
- `Result<T>` / `Result`: Pattern funcional de erro com `Error` e `ErrorType`
- `ValidationBehavior<T>`: Pipeline de validação automática via FluentValidation
- `JwtAuthenticationExtensions`: Configuração centralizada de JWT
- `QueryFilterBuilder`, `OrderByHelper`: Utilitários para filtros dinâmicos

**BuildingBlocks.Messaging:**
- `IEventBus` / `EventBus`: Publicação de eventos via RabbitMQ
- `IntegrationEvent`: Base para eventos cross-service
- `PersistentConnection`: Gerenciamento de conexão RabbitMQ com auto-reconnect
- `IEventConsumer` / `EventConsumer`: Consumo de eventos

**Core.Domain:**
- `BaseEntity<T>`: Id, CreatedAt, UpdatedAt, IsDeleted, DomainEvents
- `ValueObject`: Igualdade por valor
- `Specification<T, TId>`: Filtros composíveis com paginação e ordenação
- `DomainEvent`: Base para eventos de domínio

**Core.Application:**
- `DTOBase`: Record base com Id, CreatedAt, UpdatedAt
- `IReadService`, `ICreateService`, `IUpdateService`, `IDeleteService`: Interfaces genéricas de serviço

**Core.Infrastructure:**
- `SpecificationEvaluator`: Traduz Specifications para queries EF Core

**Core.API:**
- `ApiController`: Base controller com `[ApiController]` e `[Route("api/[controller]")]`

### Avaliação Crítica

**Pontos Fortes:**
- O `Result<T>` pattern é muito bem implementado e elimina exceções do fluxo de negócio — grande decisão arquitetural.
- O `ValidationBehavior` integra FluentValidation no pipeline CQRS automaticamente — zero boilerplate nos handlers.
- O `Specification` pattern com paginação, ordenação e includes é poderoso e reutilizável.
- `BaseEntity<T>` com suporte a domain events permite DDD verdadeiro.
- A separação entre BuildingBlocks (infraestrutura) e Core (domínio) está correta.
- `PersistentConnection` com auto-reconnect é essencial para resiliência do RabbitMQ.

**Pontos Fracos:**
- **Sem Outbox Pattern**: Domain events e integration events não garantem atomicidade. Se o save do banco succeeder mas o publish falhar, haverá inconsistência.
- **Sem Idempotency**: Consumers de eventos não verificam se já processaram um evento (risco de duplicação).
- **Sem correlation ID**: Não há rastreamento de requisições entre serviços.
- **Sem logging estruturado**: Não há configuração padrão de Serilog ou similar.
- **`HttpResult<T>` no Core.API**: Acopla a camada de API a um formato de resposta específico. Poderia ser mais flexível.

### Melhorias Sugeridas

1. **Outbox Pattern**: Implementar transactional outbox para garantir atomicidade entre banco e messaging.
2. **Idempotent Consumers**: Rastrear IDs de eventos processados para evitar duplicação.
3. **Correlation ID**: Middleware que gera e propaga correlation ID entre serviços via HTTP headers e message properties.
4. **Logging Estruturado**: Integrar Serilog com Seq ou Elastic Stack para observabilidade.
5. **Health Check Base**: Classe base de health check que verifica DB + RabbitMQ, reutilizável por todos os serviços.

---

## Análise Consolidada

### Estado Atual do Sistema

O **LifeSync** é um projeto ambicioso e bem arquitetado que demonstra maturidade em design de software. A consistência de padrões entre todos os microserviços (CQRS, Result\<T\>, Repository, Specification) evidencia um planejamento cuidadoso. O ecossistema cobre as principais áreas de bem-estar pessoal (tarefas, finanças, nutrição, treino), formando uma proposta de valor única.

### Principais Problemas Encontrados

| # | Problema | Severidade | Serviço |
|---|---|---|---|
| 1 | `Money.Amount` é `int` — impossível registrar centavos | **Crítica** | Financial |
| 2 | JWT Key hardcoded no appsettings | **Crítica** | Users/Gateway |
| 3 | Sem autorização por usuário (user A acessa dados do user B) | **Crítica** | Todos |
| 4 | UserId passado como parâmetro ao invés de extraído do JWT | **Alta** | Todos (exceto Users) |
| 5 | SMTP sem TLS (`SecureSocketOptions.None`) | **Alta** | Users/Notification |
| 6 | Testes ausentes em Financial, Nutrition e Gym | **Alta** | Financial/Nutrition/Gym |
| 7 | Sem rate limiting no gateway e endpoints de auth | **Alta** | Gateway/Users |
| 8 | JWT em localStorage (XSS) no WebApp | **Alta** | WebApp |
| 9 | Sem retry/dead letter no messaging | **Média** | Notification |
| 10 | Sem outbox pattern para eventos | **Média** | BuildingBlocks |
| 11 | MAUI ainda em .NET 9.0 (backend em 10.0) | **Média** | ClientApp |
| 12 | Sem relatórios financeiros | **Média** | Financial |
| 13 | Sem macronutrientes no progresso diário | **Média** | Nutrition |
| 14 | Sem seed de exercícios | **Média** | Gym |
| 15 | Telas de Gym ausentes no mobile | **Média** | ClientApp |

### Principais Oportunidades de Evolução

1. **Dashboard Unificado**: Tela central mostrando resumo de todos os módulos — tarefas do dia, saldo financeiro, calorias consumidas, treino programado. Seria o diferencial competitivo do LifeSync.

2. **Gamificação Cross-Module**: Sistema de streaks, badges e pontos que premia o uso consistente em todos os módulos. Ex: "5 dias consecutivos registrando refeições + completando tarefas = badge de consistência".

3. **Insights com IA**: Análise de dados cross-service para gerar insights personalizados. Ex: "Você gasta mais em delivery nos dias que não treina" ou "Sua produtividade é maior quando dorme bem e come adequadamente".

4. **Observabilidade**: Serilog + Seq/Elastic, Prometheus + Grafana, Jaeger para distributed tracing.

5. **CI/CD Completo**: Pipeline de build, test e deploy automatizado com GitHub Actions.

---

## Roadmap de Evolução

### Curto Prazo (1-2 meses) — Estabilização e Segurança

**Objetivo:** Corrigir problemas críticos de segurança e estabilidade.

- [ ] Corrigir `Money.Amount` de `int` para `decimal` (Financial)
- [ ] Mover JWT Key para variáveis de ambiente (Users/Gateway)
- [ ] Implementar autorização por usuário em todos os serviços
- [ ] Extrair UserId do JWT nos handlers (todos os serviços)
- [ ] Padronizar autorização no API Gateway (todas as rotas exceto login/register)
- [ ] Implementar rate limiting no gateway
- [ ] SMTP com TLS em produção
- [ ] Implementar testes unitários para Financial, Nutrition e Gym (domínio + handlers)
- [ ] Corrigir validação de datas passadas no Nutrition
- [ ] Migrar JWT de localStorage para HttpOnly cookie no WebApp

### Médio Prazo (3-4 meses) — Funcionalidades Core

**Objetivo:** Completar funcionalidades essenciais que dão valor real ao produto.

- [ ] Relatórios financeiros (saldo, gastos por categoria, evolução temporal)
- [ ] Dashboard financeiro com gráficos no frontend
- [ ] Macronutrientes no progresso diário (Nutrition)
- [ ] Enum MealType (Café/Almoço/Jantar) no Nutrition
- [ ] Seed de exercícios populares (Gym)
- [ ] Histórico de progressão de peso por exercício (Gym)
- [ ] Telas de Gym no MAUI
- [ ] Templates HTML para emails (Notification)
- [ ] In-app notifications (Notification)
- [ ] Retry/Dead Letter Queue no messaging
- [ ] Dashboard unificado no mobile e web
- [ ] Visualização Kanban para tarefas (TaskManager)
- [ ] Subtarefas/Checklists (TaskManager)
- [ ] OAuth/Login com Google (Users)
- [ ] Testes de integração e E2E para Financial, Nutrition, Gym

### Longo Prazo (5-8 meses) — Diferenciação e Inteligência

**Objetivo:** Tornar o LifeSync um produto diferenciado e competitivo.

- [ ] Budget (Orçamento) por categoria (Financial)
- [ ] Entidade Account/Wallet (Financial)
- [ ] Tarefas recorrentes (TaskManager)
- [ ] Plano semanal de treino (Gym)
- [ ] Timer de descanso no treino (Gym/Frontend)
- [ ] Body measurements (Gym)
- [ ] Alimentos personalizados (Nutrition)
- [ ] Integração Gym ↔ Nutrition (calorias gastas no treino)
- [ ] Push notifications mobile (FCM)
- [ ] Gamificação cross-module (streaks, badges)
- [ ] Observabilidade (Serilog + Seq, métricas com Prometheus)
- [ ] Outbox Pattern para eventos (BuildingBlocks)
- [ ] CI/CD com GitHub Actions
- [ ] PWA para WebApp
- [ ] Insights com IA (análise cross-service)
- [ ] Scanner de código de barras para alimentos (MAUI)
- [ ] 2FA / Login com biometria

---

### Sugestões para Tornar o LifeSync Mais Competitivo

1. **Proposta Única de Valor**: O diferencial do LifeSync está na **integração entre módulos**. Nenhum app popular (Todoist, Splitwise, MyFitnessPal, Strong) oferece todos esses módulos integrados. O dashboard unificado e insights cross-module são a chave.

2. **Simplicidade Acima de Tudo**: A maior ameaça a um app multifuncional é a complexidade. Cada tela deve ter um fluxo principal claro e rápido. O registro de uma despesa ou refeição deve levar no máximo 3 toques.

3. **Onboarding Progressivo**: Não mostrar todos os módulos de uma vez. Deixar o usuário ativar gradualmente (começar com tarefas, depois adicionar finanças, depois nutrição, etc.).

4. **Data Portability**: Permitir importar dados de outros apps (Todoist, Google Keep, planilhas de gastos) para reduzir barreira de migração.

5. **Design Consistente**: Usar design system unificado (cores, tipografia, componentes) em todos os módulos. A inconsistência visual entre módulos é percebida como falta de qualidade.

6. **Performance Percebida**: Investir em skeleton loaders, optimistic updates e cache local. A velocidade percebida é mais importante que a velocidade real.

7. **Comunidade**: Considerar funcionalidades sociais futuras — desafios entre amigos, compartilhamento de treinos, split de despesas.

---

> **Nota Final:** Este documento foi produzido com base na análise direta do código-fonte do repositório. Todas as funcionalidades descritas como "já implementadas" foram verificadas no código. As sugestões são baseadas em boas práticas de engenharia de software, padrões de mercado e experiência em design de produto.
