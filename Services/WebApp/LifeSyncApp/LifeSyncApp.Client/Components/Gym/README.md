# Componentes Gym

Esta pasta contém os componentes relacionados ao módulo de Academia (Gym) do LifeSync, reconstruídos com **Syncfusion Blazor Components** para uma experiência profissional e moderna.

## ??? Arquitetura

A página Gym foi completamente redesenhada usando componentes Syncfusion, seguindo uma arquitetura baseada em **Tabs** para melhor organização e usabilidade.

## ?? Componentes

### Gym.razor (Página Principal)
Componente principal que utiliza `SfTab` do Syncfusion para organizar as funcionalidades em abas:
- **Exercícios** - Gestão de exercícios
- **Rotinas** - Gestão de rotinas e construtor
- **Sessões** - Treino ativo e histórico
- **Dashboard** - Estatísticas e visualizações

### ExercisesTab.razor
Gerenciamento completo de exercícios usando `SfGrid` e `SfDialog`.

**Funcionalidades:**
- Grid com paginação, ordenação e filtros
- CRUD completo de exercícios
- Campos: Nome, Descrição, Grupo Muscular, Tipo, Equipamento
- Validação de formulário
- Confirmação de exclusão

**Componentes Syncfusion:**
- `SfGrid` - Tabela de dados
- `SfDialog` - Modal para criar/editar
- `SfButton` - Botões de ação

### RoutinesTab.razor
Gerenciamento de rotinas com construtor integrado.

**Funcionalidades:**
- Grid de rotinas com paginação e filtros
- CRUD de rotinas (Nome, Descrição)
- **Construtor de Rotinas** - Modal para montar rotinas
  - Seleção de exercícios via dropdown
  - Configuração de séries, repetições e descanso
  - Adicionar/remover exercícios da rotina
  - Lista visual dos exercícios configurados

**Componentes Syncfusion:**
- `SfGrid` - Lista de rotinas
- `SfDialog` - Modais de criação e construtor
- `SfDropDownList` - Seleção de exercícios
- `SfButton` - Botões de ação

### SessionsTab.razor
Gerenciamento de sessões de treino com execução de treino em tempo real.

**Funcionalidades:**
- **Iniciar Sessão** - Seleção de rotina via dropdown
- **Treino Ativo:**
  - Cards de exercícios com informações planejadas
  - Inputs para registrar valores realizados (séries, reps, peso)
  - Marcar exercícios como concluídos
  - Indicadores visuais de progresso
- **Finalizar Sessão** - Encerra e salva a sessão
- **Histórico:**
  - Grid com todas as sessões
  - Status (Concluída/Em Andamento)
  - Duração calculada
  - Visualização e exclusão

**Componentes Syncfusion:**
- `SfGrid` - Histórico de sessões
- `SfDialog` - Modal para iniciar sessão
- `SfDropDownList` - Seleção de rotina
- `SfButton` - Ações e controles

### GymDashboardTab.razor
Dashboard com estatísticas e visualizações dos treinos.

**Funcionalidades:**
- **Cards de Estatísticas:**
  - Total de treinos
  - Treinos da semana
  - Total de rotinas
  - Total de exercícios
- **Últimas Sessões** - Lista das 5 sessões mais recentes
- **Rotinas Mais Usadas** - Ranking de uso

## ?? Design e UX

### Cores e Temas
- **Primary (Azul)** - Ações principais
- **Success (Verde)** - Exercícios completados, estatísticas positivas
- **Warning (Amarelo)** - Sessões em andamento
- **Info (Azul claro)** - Informações e visualizações
- **Danger (Vermelho)** - Exclusões e ações destrutivas

### Responsividade
Todos os componentes são responsivos e se adaptam a diferentes tamanhos de tela:
- Grid com colunas ajustáveis
- Cards em grid responsivo (col-md, col-lg)
- Modais centralizados e adaptáveis

## ?? Fluxo de Trabalho Completo

### 1. Criar Exercícios
```
Exercícios Tab ? Novo Exercício ? Preencher formulário ? Criar
```

### 2. Criar e Configurar Rotina
```
Rotinas Tab ? Nova Rotina ? Preencher dados ? Criar
               ?
         Selecionar Rotina ? Configurar
               ?
         Adicionar Exercícios ? Configurar séries/reps/descanso ? Adicionar
```

### 3. Realizar Treino
```
Sessões Tab ? Iniciar Sessão ? Selecionar Rotina ? Iniciar Treino
                ?
         Executar Exercícios ? Ajustar valores ? Marcar Concluído
                ?
         Finalizar Sessão
```

### 4. Acompanhar Progresso
```
Dashboard Tab ? Visualizar estatísticas e histórico
```

## ??? Tecnologias

### Syncfusion Components
- **Syncfusion.Blazor.Grid** - Grids de dados
- **Syncfusion.Blazor.Buttons** - Botões e ações
- **Syncfusion.Blazor.Popups** - Modais e diálogos
- **Syncfusion.Blazor.DropDowns** - Dropdowns e seleções
- **Syncfusion.Blazor.Navigations** - Tabs e navegação
- **Syncfusion.Blazor.Themes** - Temas visuais

### .NET 9
- Blazor WebAssembly
- C# 13.0
- Async/Await patterns
- Dependency Injection

## ?? Modelos de Dados

### ExerciseDTO
- Id, Name, Description
- MuscleGroup, ExerciseType, EquipmentType
- CreatedAt, UpdatedAt

### RoutineDTO
- Id, Name, Description
- CreatedAt, UpdatedAt

### RoutineExerciseDTO
- Id, RoutineId, ExerciseId
- Sets, Repetitions, RestBetweenSets
- RecommendedWeight, Instructions

### TrainingSessionDTO
- Id, UserId, RoutineId
- StartTime, EndTime, Notes
- CreatedAt, UpdatedAt

### CompletedExerciseDTO
- Id, TrainingSessionId, RoutineExerciseId
- SetsCompleted, RepetitionsCompleted
- WeightUsed, Notes

## ? Performance

- **Lazy Loading** - Dados carregados sob demanda
- **Virtual Scrolling** - Grid com virtualização (disponível)
- **Async Operations** - Todas operações são assíncronas
- **State Management** - Gerenciamento eficiente de estado

## ?? Melhorias Futuras

- [ ] Gráficos de progresso com Syncfusion Charts
- [ ] Filtros avançados de data
- [ ] Exportação de dados (Excel, PDF)
- [ ] Templates de rotinas pré-configuradas
- [ ] Histórico detalhado de exercícios
- [ ] Análise de volume de treino
- [ ] Metas e objetivos

## ?? Notas de Desenvolvimento

- Todos os componentes usam injeção de dependência para `IGymService`
- Confirmações de exclusão via `JSRuntime.InvokeAsync`
- Tratamento de erros com try-catch
- Feedback visual de loading states
- Validação de formulários antes de submissão

---

**LifeSync Gym Module** - Versão 2.0 com Syncfusion ??????
