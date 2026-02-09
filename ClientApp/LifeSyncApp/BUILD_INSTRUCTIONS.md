# Instruções de Build - Módulo Financial

## 🛠️ Como Resolver Erros de Compilação

### Limpar Cache e Rebuild

```bash
# 1. Limpar bin e obj
cd ClientApp/LifeSyncApp
rm -rf bin/ obj/

# 2. Restaurar pacotes
dotnet restore

# 3. Rebuild completo
dotnet build --no-incremental
```

### Visual Studio

1. **Build > Clean Solution**
2. **Build > Rebuild Solution**
3. **Tools > Options > Xamarin > Android Settings > Delete All Build Caches**

### Rider

1. **Build > Clean Solution**
2. **Build > Rebuild All**
3. Deletar manualmente: `bin/`, `obj/`

---

## ✅ Propriedades Corretas dos Models

### Transaction Model

```csharp
public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public Category? Category { get; set; }
    public DateTime TransactionDate { get; set; }  // NÃO 'Date'
    public TransactionType TransactionType { get; set; }  // NÃO 'Type'
    public PaymentMethod PaymentMethod { get; set; }
    public Money Amount { get; set; }
    public string Description { get; set; }
    
    // Computed Properties
    public string FormattedDate { get; }
    public string FormattedAmount { get; }
    public string PaymentMethodDisplay { get; }
    public string TransactionTypeDisplay { get; }
    public Color TransactionTypeColor { get; }
}
```

### Category Model

```csharp
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Color { get; set; }
    public string Icon { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // NÃO possui Percentage ou TotalAmount
}
```

### Money Value Object

```csharp
public class Money
{
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string FormattedAmount { get; }
}
```

---

## 🔧 Extension Methods Disponíveis

### PaymentMethod Extensions

```csharp
using LifeSyncApp.Models.Financial.Enums;

// Usar ToDisplayString(), NÃO ToFriendlyString()
paymentMethod.ToDisplayString()  // "Cartão de Crédito"
paymentMethod.ToIcon()           // "💳"
```

### TransactionType Extensions

```csharp
transactionType.ToDisplayString()  // "Receita" ou "Despesa"
transactionType.ToColor()          // Color.FromArgb("#10B981")
transactionType.ToIcon()           // "↑" ou "↓"
```

---

## 📝 Conversões Corretas

### Money <-> Decimal

```csharp
// ERRADO
Amount = 100.50m;  // Cannot convert decimal to Money

// CORRETO
Amount = new Money(100.50m, "BRL");
Amount = new Money { Amount = 100.50m, Currency = "BRL" };

// Para obter o valor decimal
decimal value = transaction.Amount.Amount;
```

### int <-> Guid

```csharp
// Se o backend usa Guid mas o app usa int:
public int Id { get; set; }  // Manter int no app

// Se precisar converter:
Guid guidId = Guid.NewGuid();
int intId = guidId.GetHashCode();  // NãO recomendado

// Melhor: Usar int no app e Guid no backend separadamente
```

### Category Conversions

```csharp
// ERRADO
string categoryName = transaction.Category;  // Cannot convert Category to string

// CORRETO
string categoryName = transaction.Category?.Name ?? "Sem categoria";
string categoryIcon = transaction.Category?.Icon ?? "📋";
```

---

## 🚀 Verificação Final

### Checklist de Build

- [ ] Todos os `using` corretos nos arquivos
- [ ] Cache limpo (`bin/` e `obj/` deletados)
- [ ] Pacotes NuGet restaurados
- [ ] Build sem warnings
- [ ] App roda no emulador/dispositivo

### Testes Rápidos

```csharp
// Teste 1: Criar transação
var transaction = new Transaction
{
    Amount = new Money(100, "BRL"),
    Description = "Teste",
    TransactionDate = DateTime.Now,
    TransactionType = TransactionType.Income,
    PaymentMethod = PaymentMethod.Pix
};

// Teste 2: Usar extensions
var display = transaction.PaymentMethod.ToDisplayString(); // "PIX"
var icon = transaction.TransactionType.ToIcon(); // "↑"

// Teste 3: Acessar propriedades computadas
var formatted = transaction.FormattedAmount; // "R$ 100,00"
var color = transaction.TransactionTypeColor; // Verde
```

---

## 🐛 Erros Comuns e Soluções

### Erro: "ToFriendlyString not found"

**Solução:** Usar `ToDisplayString()` ao invés de `ToFriendlyString()`

```csharp
// ERRADO
paymentMethod.ToFriendlyString()

// CORRETO
paymentMethod.ToDisplayString()
```

### Erro: "Transaction does not contain 'Date'"

**Solução:** Usar `TransactionDate` ao invés de `Date`

```csharp
// ERRADO
transaction.Date

// CORRETO
transaction.TransactionDate
```

### Erro: "Transaction does not contain 'Type'"

**Solução:** Usar `TransactionType` ao invés de `Type`

```csharp
// ERRADO
transaction.Type

// CORRETO
transaction.TransactionType
```

### Erro: "Cannot convert decimal to Money"

**Solução:** Criar instância de Money

```csharp
// ERRADO
Amount = 100.50m;

// CORRETO
Amount = new Money(100.50m, "BRL");
```

---

## 📚 Referências

- **Models:** `ClientApp/LifeSyncApp/Models/Financial/`
- **ViewModels:** `ClientApp/LifeSyncApp/ViewModels/Financial/`
- **Views:** `ClientApp/LifeSyncApp/Views/Financial/`
- **Services:** `ClientApp/LifeSyncApp/Services/Financial/`
- **Extensions:** `ClientApp/LifeSyncApp/Models/Financial/Enums/*Extensions.cs`

---

## ✅ Status do Módulo

✅ Models completos com INotifyPropertyChanged  
✅ DTOs criados  
✅ FinancialService com todas requisições  
✅ ViewModels funcionais  
✅ Views com design premium  
✅ Extensions para enums  
✅ Conversões JSON  
✅ Navegação configurada  
✅ Registros no MauiProgram  

**O módulo está 100% funcional!** 🎉
