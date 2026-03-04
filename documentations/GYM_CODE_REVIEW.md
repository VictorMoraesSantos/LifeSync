# Gym Microservice - Code Review Completo

> **Autor:** Code Review Senior
> **Data:** 03/03/2026
> **Escopo:** Varredura completa de todas as camadas (Domain, Application, Infrastructure, API)
> **Severidade:** CRITICO | ALTO | MEDIO | BAIXO | INFO

---

## Sumario Executivo

O microservice Gym gerencia exercicios, rotinas, sessoes de treino e exercicios completados. Possui bom uso de Value Objects (SetCount, RepetitionCount, Weight, RestTime, Duration, ExerciseIntensity). Porem apresenta **bugs criticos no Mapper**, **problemas graves de performance**, **falhas de validacao nas Specifications** e **ausencia de isolamento de dados por usuario**.

### Nota Geral: 5/10

---

## 1. DOMAIN LAYER

### 1.1 [ALTO] Inconsistencia de Excecoes no Domain

```csharp
// Exercise.cs usa ArgumentException:
private void Validate(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("Value cannot be null or empty.", nameof(value));
}

// Routine.cs usa DomainException:
throw new DomainException(RoutineErrors.InvalidName);
```

**Impacto:** Handlers capturam `Exception` generica, mascando o tipo. Padroes diferentes no mesmo dominio.

**Correcao:** Padronizar para `DomainException` em todas as entidades.

---

### 1.2 [ALTO] CompletedAt Nunca Inicializado

**Arquivo:** `CompletedExercise.cs`

```csharp
public DateTime CompletedAt { get; private set; } // Nunca setado no construtor!
```

O campo `CompletedAt` fica com `default(DateTime)` (01/01/0001). O metodo `MarkCompleted()` existe mas nunca e chamado pelos handlers.

**Impacto:** Exercicios completados tem data invalida no banco.

**Correcao:** Inicializar `CompletedAt = DateTime.UtcNow` no construtor ou chamar `MarkCompleted()` no handler.

---

### 1.3 [MEDIO] Colecao Nullable Desnecessaria

```csharp
private readonly List<CompletedExercise?> _completedExercises = new();
public IReadOnlyCollection<CompletedExercise?> CompletedExercises
```

**Impacto:** Items nullable na colecao criam confusao e checks desnecessarios.

**Correcao:** Remover `?` do tipo generico.

---

### 1.4 [MEDIO] Domain Events Comentados

```csharp
// AddDomainEvent(new TrainingSessionCompletedEvent(UserId, Id));
```

Eventos de dominio existem no codigo mas estao comentados. A infraestrutura de eventos esta ociosa.

---

### 1.5 [BAIXO] Mensagens de Erro em Idiomas Misturados

- Ingles: "Exercise name cannot be null or empty"
- Portugues: "O nome da rotina e obrigatorio"

**Correcao:** Padronizar para um unico idioma.

---

## 2. APPLICATION LAYER

### 2.1 [CRITICO] Bug no RoutineMapper - Parametros Invertidos

**Arquivo:** `RoutineMapper.cs`

```csharp
public static RoutineDTO ToDTO(this Routine entity)
{
    var dto = new RoutineDTO(
        entity.Id,
        entity.CreatedAt,
        entity.UpdatedAt,
        entity.Description,  // ERRADO! Deveria ser Name
        entity.Name);        // ERRADO! Deveria ser Description
}
```

O DTO espera `(Id, CreatedAt, UpdatedAt, Name, Description)` mas o mapper passa `(Description, Name)` na ordem invertida.

**Impacto:** Nome e descricao de TODAS as rotinas estao trocados na API. Bug critico em producao.

**Correcao:** Inverter a ordem dos parametros.

---

### 2.2 [MEDIO] Camada Service Redundante

O fluxo atual e:
```
Controller -> Handler -> Service -> Repository
```

Os handlers sao thin wrappers que apenas transformam DTO e delegam ao service. Poderia simplificar eliminando uma camada.

---

### 2.3 [BAIXO] Arquivos com Extensao Duplicada

- `UpdateRoutineExerciseDTO.cs.cs`
- `UpdateTrainingSessionDTO.cs.cs`

**Correcao:** Renomear removendo a extensao duplicada.

---

## 3. INFRASTRUCTURE LAYER

### 3.1 [CRITICO] Paginacao Em Memoria

**Arquivos:** `ExerciseService.cs`, `RoutineService.cs`, `TrainingSessionService.cs`, `CompletedExerciseService.cs`

```csharp
var entities = await _exerciseRepository.GetAll(cancellationToken); // CARREGA TUDO
var totalCount = entities.Count();
var items = entities.Skip((page - 1) * pageSize).Take(pageSize);
```

**Impacto:** Todas as 4 entidades principais sofrem do mesmo problema. Performance O(n) em toda listagem.

---

### 3.2 [CRITICO] Bug nas Specifications - Semantica Incorreta

**Arquivo:** `RoutineExerciseSpecification.cs`

```csharp
AddIf(filter.SetsEquals.HasValue, re => re.Sets.Value <= filter.SetsEquals!.Value);
```

O filtro chama-se `SetsEquals` mas usa `<=` (menor ou igual) ao inves de `==` (igual).

**Impacto:** Usuarios buscando "exercicios com 5 sets" recebem exercicios com 1, 2, 3, 4 e 5 sets. Mesma issue com `RecommendedWeight`.

**Correcao:** Mudar para `==` ou renomear filtro para `SetsLessThanOrEqual`.

---

### 3.3 [ALTO] NullReferenceException na TrainingSessionSpecification

```csharp
AddIf(!string.IsNullOrWhiteSpace(filter.NotesContains),
    r => r.Notes.Contains(filter.NotesContains!));
```

Se `Notes` for null no banco, `.Contains()` lanca `NullReferenceException`.

**Correcao:**
```csharp
r => r.Notes != null && r.Notes.Contains(filter.NotesContains!)
```

---

### 3.4 [ALTO] Duplicata na CompletedExerciseConfiguration

```csharp
builder.OwnsOne(ce => ce.SetsCompleted);
builder.OwnsOne(ce => ce.RepetitionsCompleted);
builder.OwnsOne(ce => ce.SetsCompleted);  // DUPLICATA!
builder.OwnsOne(ce => ce.WeightUsed);
```

`SetsCompleted` configurado duas vezes.

---

### 3.5 [ALTO] Ausencia de Indexes

Nenhum index criado via Fluent API para:
- `Exercise.Name`
- `TrainingSession.UserId`
- `TrainingSession.RoutineId`
- `CompletedExercise.TrainingSessionId`

---

### 3.6 [ALTO] Sem Isolamento de Dados por Usuario

```csharp
public async Task<Result<IEnumerable<TrainingSessionDTO?>>> GetByUserIdAsync(int userId, ...)
```

Nenhuma validacao de que o usuario autenticado e o dono dos dados. Qualquer usuario pode consultar treinos de outro.

---

### 3.7 [ALTO] FindAsync Carrega Tudo em Memoria

Mesmo padrao do Financial/TaskManager:

```csharp
var entities = await _exerciseRepository.GetAll(cancellationToken);
var dtos = entities.Select(e => e.ToDTO()).AsQueryable().Where(predicate).ToList();
```

---

### 3.8 [MEDIO] Eager Loading Inconsistente

- ExerciseRepository: Nenhum include
- RoutineRepository: Inclui `RoutineExercises`
- TrainingSessionRepository: Inclui `Routine` e `CompletedExercises`

Sem estrategia clara de quando carregar relacoes.

---

### 3.9 [MEDIO] Sem Unit of Work

Cada repository chama `SaveChangesAsync()` individualmente. Operacoes como "criar rotina + adicionar exercicios" nao sao atomicas.

---

## 4. API LAYER

### 4.1 [CRITICO] Sem [Authorize] nos Controllers

Endpoints publicos sem autenticacao.

---

### 4.2 [CRITICO] JWT Secret Hardcoded

```json
"Key": "SuperSecretKeyForJWTAuthentication2024!@#$%"
```

---

### 4.3 [ALTO] Validacao de ID Route vs Body Ausente

```csharp
public async Task<HttpResult<object>> Update(
    int id, [FromBody] UpdateExerciseCommand command, ...)
{
    var updatedCommand = new UpdateExerciseCommand(id, command.Name, ...);
}
```

Se `command.Id` diferir do route `id`, nao ha erro. O ID do route sobrescreve silenciosamente.

---

## 5. PLANO DE ACAO PRIORIZADO

### Prioridade 1 - Criticos
| # | Item | Esforco |
|---|------|---------|
| 1 | Corrigir RoutineMapper parametros invertidos | 10 min |
| 2 | Adicionar [Authorize] nos controllers | 10 min |
| 3 | Remover JWT Key do appsettings.json | 30 min |
| 4 | Corrigir paginacao em memoria (4 services) | 4h |
| 5 | Corrigir SetsEquals specification (== ao inves de <=) | 15 min |

### Prioridade 2 - Altos
| # | Item | Esforco |
|---|------|---------|
| 6 | Corrigir NullReference na TrainingSessionSpecification | 15 min |
| 7 | Remover duplicata CompletedExerciseConfiguration | 10 min |
| 8 | Adicionar indexes no banco | 30 min |
| 9 | Implementar isolamento de dados por usuario | 2h |
| 10 | Inicializar CompletedAt no construtor | 15 min |
| 11 | Padronizar excecoes para DomainException | 1h |
| 12 | Corrigir FindAsync para filtrar no banco | 2h |

### Prioridade 3 - Medios
| # | Item | Esforco |
|---|------|---------|
| 13 | Implementar Unit of Work | 4h |
| 14 | Definir estrategia de Eager Loading | 2h |
| 15 | Remover nullable da colecao CompletedExercises | 15 min |
| 16 | Habilitar Domain Events | 2h |
| 17 | Padronizar idioma das mensagens | 1h |
| 18 | Corrigir extensao duplicada dos arquivos | 5 min |

---

## Resumo Final

| Severidade | Quantidade |
|------------|-----------|
| CRITICO | 5 |
| ALTO | 7 |
| MEDIO | 6 |
| BAIXO | 2 |
