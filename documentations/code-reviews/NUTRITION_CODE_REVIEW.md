# Nutrition Microservice - Code Review Completo

> **Autor:** Code Review Senior
> **Data:** 03/03/2026
> **Escopo:** Varredura completa de todas as camadas (Domain, Application, Infrastructure, API)
> **Severidade:** CRITICO | ALTO | MEDIO | BAIXO | INFO

---

## Sumario Executivo

O microservice Nutrition e o mais complexo do sistema, gerenciando Diaries, Meals, Foods, Liquids, MealFoods e DailyProgress. Possui bom uso de Domain Events (MealAddedToDiaryEvent, MealFoodAddedEvent, MealFoodRemovedEvent) e Value Object (DailyGoal). Porem apresenta **entidade Food totalmente anemica**, **bugs de NullReference**, **problemas graves de performance** e **ausencia de validacao de input**.

### Nota Geral: 5.5/10

---

## 1. DOMAIN LAYER

### 1.1 [CRITICO] Entidade Food Totalmente Anemica

**Arquivo:** `Food.cs`

```csharp
public class Food : BaseEntity<int>
{
    public string Name { get; set; }       // PUBLIC SETTER!
    public int Calories { get; set; }      // PUBLIC SETTER!
    public decimal? Protein { get; set; }  // PUBLIC SETTER!
    public decimal? Carbs { get; set; }    // PUBLIC SETTER!
    public decimal? Fat { get; set; }      // PUBLIC SETTER!
    // ... 5+ propriedades com setters publicos
}
```

**Impacto:**
- Zero encapsulamento - qualquer camada pode modificar diretamente
- Sem validacao - Name pode ser null, Calories pode ser negativo
- Viola Rich Domain Model que as outras entidades seguem

**Correcao:** Adicionar private setters, construtor com validacao, metodos Set*.

---

### 1.2 [CRITICO] Bug de NullReference no Meal.RemoveMealFood()

**Arquivo:** `Meal.cs`

```csharp
public void RemoveMealFood(int mealFoodId)
{
    if (mealFoodId == null)  // BUG: int NUNCA e null!
        throw new DomainException(MealErrors.NullMealFood);

    var mealFood = _mealFoods.FirstOrDefault(mf => mf.Id == mealFoodId);
    _mealFoods.Remove(mealFood);  // Remove null se nao encontrar!

    AddDomainEvent(new MealFoodRemovedEvent(DiaryId, mealFood.TotalCalories));
    // NullReferenceException em mealFood.TotalCalories se nao encontrado!
}
```

**Impacto:** Se o `mealFoodId` nao existir na colecao:
1. `FirstOrDefault` retorna null
2. `Remove(null)` silenciosamente nao faz nada
3. `mealFood.TotalCalories` lanca `NullReferenceException`

**Correcao:**
```csharp
public void RemoveMealFood(int mealFoodId)
{
    var mealFood = _mealFoods.FirstOrDefault(mf => mf.Id == mealFoodId)
        ?? throw new DomainException(MealErrors.MealFoodNotFound);

    _mealFoods.Remove(mealFood);
    AddDomainEvent(new MealFoodRemovedEvent(DiaryId, mealFood.TotalCalories));
}
```

---

### 1.3 [ALTO] Propriedade Computada Sem Cache - N+1 Potencial

**Arquivo:** `Meal.cs`

```csharp
public int TotalCalories => _mealFoods?.Sum(i => i.Food.Calories * i.Quantity) ?? 0;
```

**Impacto:**
1. Recalculado a cada acesso (sem cache)
2. Acessa `i.Food.Calories` - se Food nao tiver eager loading, dispara N+1 queries
3. Usado em `Diary.TotalCalories` que soma os `Meal.TotalCalories` (cascata de N+1)

**Correcao:** Cachear o valor e recalcular apenas quando MealFoods mudar.

---

### 1.4 [MEDIO] LiquidType com Falha Silenciosa

**Arquivo:** `LiquidType.cs`

```csharp
public LiquidType(string name)
{
    if (!string.IsNullOrEmpty(name))
        Name = name;
    // Se name for vazio, Name fica null! Sem excecao!
}
```

**Correcao:** Lancar excecao se nome for vazio.

---

### 1.5 [INFO] Domain Events Bem Implementados

O Nutrition e o unico microservice que **efetivamente usa Domain Events**:
- `MealAddedToDiaryEvent` - atualiza DailyProgress quando meal e adicionada
- `MealFoodAddedEvent` - recalcula calorias quando food e adicionada
- `MealFoodRemovedEvent` - recalcula calorias quando food e removida

Bom padrao que deveria ser replicado nos outros microservices.

---

## 2. APPLICATION LAYER

### 2.1 [ALTO] Ausencia de FluentValidation

Nenhum command possui validator registrado. Validacao acontece apenas no domain e nos services, sem validacao na entrada da API.

**Impacto:** Requests invalidos chegam ate a camada de domain, gerando DomainExceptions ao inves de respostas 400 amigaveis.

**Correcao:** Adicionar `AbstractValidator<T>` para cada command.

---

### 2.2 [ALTO] Event Handlers com Falha Silenciosa

**Arquivo:** `MealFoodAddedEventHandler.cs`

```csharp
public async Task Handle(MealFoodAddedEvent notification, ...)
{
    var diary = await _diaryRepository.GetById(notification.DiaryId, ...);
    if (diary == null) return;  // FALHA SILENCIOSA!

    var dailyProgress = await _dailyProgressRepository.GetByUserIdAndDateAsync(...);
    if (dailyProgress == null) return;  // FALHA SILENCIOSA NOVAMENTE!
}
```

**Impacto:** Se o Diary ou DailyProgress nao existir, o calculo de calorias nao e atualizado e ninguem e notificado.

**Correcao:** Logar warning e/ou lancar excecao.

---

### 2.3 [MEDIO] Inconsistencia de Namespaces

```csharp
// IFoodService em:
namespace Nutrition.Application.Contracts

// IDiaryService em:
namespace Nutrition.Application.Interfaces
```

**Correcao:** Consolidar em um unico namespace.

---

## 3. INFRASTRUCTURE LAYER

### 3.1 [CRITICO] Paginacao Em Memoria (3 Services)

**Arquivos:** `DailyProgressService.cs`, `DiaryService.cs`, `MealService.cs`

Mesmo padrao critico encontrado nos outros microservices:

```csharp
var entities = await _dailyProgressRepository.GetAll(cancellationToken);
var totalCount = entities.Count();
var items = entities.Skip((page - 1) * pageSize).Take(pageSize);
```

---

### 3.2 [CRITICO] FindAsync Carrega Tudo em Memoria

```csharp
var entities = await _dailyProgressRepository.GetAll(cancellationToken);
var dtos = entities
    .Select(DailyProgressMapper.ToDTO)
    .AsQueryable()
    .Where(predicate) // Filtragem EM MEMORIA sobre DTOs
    .ToList();
```

---

### 3.3 [ALTO] Specification Pagination Perdida

**Arquivo:** `DiaryRepository.cs`

```csharp
var spec = new DiarySpecification(filter);
IQueryable<Diary> countQuery = spec.Criteria != null
    ? query.Where(spec.Criteria) : query;
int totalCount = await countQuery.CountAsync(cancellationToken);

// Cria NOVA query sem aplicar pagination do spec!
IQueryable<Diary> finalQuery = SpecificationEvaluator.GetQuery(
    _context.Diaries.AsNoTracking(), spec);
```

**Impacto:** A paginacao definida na specification pode estar sendo perdida na avaliacao final.

---

### 3.4 [ALTO] Ausencia de Indexes

Nenhum index visivel para:
- `DailyProgress.UserId`
- `DailyProgress.Date` (ou composto UserId+Date)
- `Diary.UserId`
- `Meal.DiaryId`
- `Liquid.DiaryId`
- `MealFood.MealId`

---

### 3.5 [ALTO] Sem Isolamento de Dados por Usuario

Controllers aceitam `userId` como parametro sem validar contra o JWT. Qualquer usuario autenticado pode acessar diarios e progresso de outro usuario.

---

### 3.6 [ALTO] Sem Global Query Filter para Soft Delete

Entidades possuem (ou deveriam possuir) `IsDeleted` mas nao ha filtro global.

---

### 3.7 [MEDIO] Sem Unit of Work

Operacoes como "adicionar meal ao diary + atualizar daily progress" deveriam ser atomicas, mas cada repository salva independentemente.

---

### 3.8 [BAIXO] Typo no Nome do Arquivo

```
DiaryService..cs  // Ponto duplo no nome do arquivo
```

---

## 4. API LAYER

### 4.1 [CRITICO] Sem [Authorize] nos Controllers

Endpoints publicos sem autenticacao.

---

### 4.2 [CRITICO] JWT Secret Hardcoded

Mesmo padrao dos outros microservices.

---

### 4.3 [ALTO] Sem Global Exception Handler

Apenas `ValidationException` tratada. `DomainException` e outras propagam com stack trace.

---

## 5. PLANO DE ACAO PRIORIZADO

### Prioridade 1 - Criticos
| # | Item | Esforco |
|---|------|---------|
| 1 | Encapsular entidade Food (private setters + validacao) | 2h |
| 2 | Corrigir NullReference no Meal.RemoveMealFood() | 15 min |
| 3 | Adicionar [Authorize] nos controllers | 10 min |
| 4 | Remover JWT Key do appsettings.json | 30 min |
| 5 | Corrigir paginacao em memoria (3 services) | 3h |
| 6 | Corrigir FindAsync para filtrar no banco | 2h |

### Prioridade 2 - Altos
| # | Item | Esforco |
|---|------|---------|
| 7 | Adicionar FluentValidation nos commands | 4h |
| 8 | Corrigir event handlers com falha silenciosa | 1h |
| 9 | Adicionar indexes no banco | 30 min |
| 10 | Implementar isolamento de dados por usuario | 2h |
| 11 | Adicionar Global Query Filter | 30 min |
| 12 | Corrigir specification pagination | 1h |
| 13 | Adicionar Global Exception Handler | 2h |

### Prioridade 3 - Medios
| # | Item | Esforco |
|---|------|---------|
| 14 | Cachear TotalCalories computado | 2h |
| 15 | Implementar Unit of Work | 4h |
| 16 | Consolidar namespaces (Contracts vs Interfaces) | 30 min |
| 17 | Corrigir LiquidType falha silenciosa | 15 min |
| 18 | Corrigir typo no nome do arquivo | 5 min |

---

## Resumo Final

| Severidade | Quantidade |
|------------|-----------|
| CRITICO | 6 |
| ALTO | 7 |
| MEDIO | 5 |
| BAIXO | 1 |
| INFO | 1 |
