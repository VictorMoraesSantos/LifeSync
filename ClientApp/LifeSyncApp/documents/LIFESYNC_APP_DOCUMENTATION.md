# LifeSyncApp - Documentação Técnica

## 1. Visão Geral do Projeto

**LifeSyncApp** é um aplicativo móvel multiplataforma desenvolvido em **.NET MAUI** (Microsoft Multi-platform App UI) que oferece gestão integrada de finanças pessoais, nutrição, tarefas e perfil do usuário.

### Características Técnicas Principais
- **Framework**: .NET MAUI (net10.0)
- **Arquitetura**: MVVM com CommunityToolkit.Mvvm
- **Autenticação**: JWT Token-based
- **API Base URL**: https://api.lifesync.tech
- **Idiomas**: Interface em Português (Brasil)

### Plataformas Suportadas
| Plataforma | Requisito Mínimo |
|------------|------------------|
| Android | API 21+ |
| iOS | iOS 15+ |
| Mac Catalyst | Mac Catalyst 15+ |
| Windows | Windows 10 (19041+) |

---

## 2. Arquitetura do Projeto

### Estrutura de Pastas

```
LifeSyncApp/
├── Auth/                    # ViewModels de autenticação
├── DTOs/                    # Data Transfer Objects
│   ├── Auth/
│   ├── Common/
│   ├── Financial/
│   ├── Nutrition/
│   ├── Profile/
│   └── TaskManager/
├── Helpers/                 # Classes auxiliares
├── Mapping/                 # Extensões de mapeamento
├── Messages/                # Sistema de mensagens MVVM
├── Models/                  # Modelos de domínio
│   ├── Financial/
│   └── TaskManager/
├── Services/                # Serviços de negócio
├── ViewModels/              # ViewModels (estado e lógica)
│   ├── Auth/
│   ├── Financial/
│   ├── Nutrition/
│   ├── Profile/
│   └── TaskManager/
├── Views/                   # Páginas e controles
│   ├── Auth/
│   ├── Financial/
│   ├── Nutrition/
│   ├── Profile/
│   └── TaskManager/
├── Controls/                # Controles customizados
├── Converters/              # Conversores de dados XAML
├── App.xaml / App.xaml.cs   # Ponto de entrada
├── AppShell.xaml / .cs      # Shell de navegação
├── MainPage.xaml / .cs      # Página principal com tabs
└── MauiProgram.cs           # Configuração DI
```

---

## 3. Stack Tecnológico

### NuGet Dependencies

| Pacote | Versão | Propósito |
|--------|--------|-----------|
| CommunityToolkit.Mvvm | 8.4.0 | Infraestrutura MVVM |
| Microsoft.Extensions.Http | 10.0.1 | HTTP Client Factory & DI |
| Microsoft.Maui.Controls | $(MauiVersion) | Framework MAUI |
| System.IdentityModel.Tokens.Jwt | 8.16.0 | Manipulação de JWT |

---

## 4. Módulos de Funcionalidades

### 4.1 Módulo Financeiro (Financial)

#### Models
- **Money**: Representa valores monetários (armazenado como int em centavos)
- **TransactionGroup**: Agrupa transações por data
- **CategoryExpense**: Breakdown de despesas por categoria
- **SelectableCategoryItem**: Seleção de categoria na UI
- **SelectablePaymentMethodItem**: Seleção de método de pagamento

#### Enums
- **RecurrenceFrequency**: Daily, Weekly, Monthly, Yearly
- **Currency**: USD, EUR, BRL
- **TransactionType**: Income, Expense
- **PaymentMethod**: Cash, CreditCard, DebitCard, BankTransfer, Pix, Other

#### DTOs
| DTO | Descrição |
|-----|-----------|
| TransactionDTO | Transação completa com categoria, método, valor |
| CreateTransactionDTO | Criação com parâmetros de recorrência |
| TransactionFilterDTO | Filtros complexos (data, valor, categoria, ordenação, paginação) |
| CategoryDTO | Categoria somente leitura |
| RecurrenceScheduleDTO | Info de agendamento recorrente |

#### Views
- **FinancialPage**: Dashboard principal com saldo, receitas/despesas, categorias, transações recentes
- **TransactionListPage**: Lista de transações agrupadas por data
- **TransactionDetailModal**: Detalhes da transação com info de recorrência
- **ManageTransactionModal**: Criar/editar transação
- **FilterTransactionModal**: Filtros de transação
- **CategoriesPage**: CRUD de categorias
- **ManageCategoryModal**: Criar/editar categoria

---

### 4.2 Módulo Nutrição (Nutrition)

#### DTOs
| DTO | Descrição |
|-----|-----------|
| DiaryDTO | Diário diário com refeições e líquidos |
| MealDTO | Refeição com lista de alimentos e total calórico |
| MealFoodDTO | Alimento com info nutricional e valores escalados |
| FoodDTO | Item alimentar (calorias, proteínas, lipídios, carboidratos, cálcio, magnésio, ferro, sódio, potássio) |
| LiquidDTO | Registro de líquido consumido |
| LiquidTypeDTO | Tipo de líquido (café, chá, água) |
| DailyProgressDTO | Progresso diário com calorias e hidratação |
| DailyGoalDTO | Meta calórica e de hidratação |

#### Views
- **NutritionPage**: Dashboard com progresso circular de calorias/hidratação, macros, refeições, líquidos
- **DailyProgressPage**: Progresso detalhado com navegação de datas e histórico
- **MealDetailPage**: Detalhes de refeição com alimentos
- **FoodSearchPage**: Busca de alimentos com sheet inferior para seleção
- **ManageMealModal**: Criar/editar refeição
- **ManageLiquidModal**: Adicionar/editar líquido com quantidades rápidas
- **CreateDiaryModal**: Criar novo diário
- **DiaryHistoryPage**: Histórico com paginação
- **DiaryDetailPage**: Detalhes do diário

---

### 4.3 Módulo Gerenciador de Tarefas (Task Manager)

#### Models
- **TaskItem**: Tarefa com título, descrição, status, prioridade, data de vencimento, labels
- **TaskLabel**: Label colorido para categorização
- **TaskGroup**: Grupo de tarefas por data de vencimento
- **SelectableLabelItem**: Wrapper para seleção de label

#### Enums
- **Status**: Pending, InProgress, Completed, Cancelled
- **Priority**: Low, Medium, High, Urgent
- **LabelColor**: Red, Green, Blue, Yellow, Purple, Orange, Pink, Brown, Gray

#### DTOs
| DTO | Descrição |
|-----|-----------|
| TaskItemDTO | Tarefa completa com labels |
| TaskLabelDTO | Label com cor |
| FilterTaskItemDTO | Filtros por status, prioridade, data, label |

#### Views
- **TaskItemPage**: Lista principal de tarefas agrupadas por data
- **TaskItemDetailPage**: Detalhes da tarefa
- **ManageTaskItemModal**: Criar/editar tarefa
- **FilterTaskItemPopup**: Filtros de tarefa
- **TaskLabelPage**: Gerenciamento de labels
- **ManageTaskLabelModal**: Criar/editar label com color picker

---

### 4.4 Módulo de Perfil (Profile)

#### DTOs
| DTO | Descrição |
|-----|-----------|
| UserDTO | Dados do usuário (Id, FirstName, LastName, FullName, Email) |
| AuthResult | Token de acesso, refresh token e dados do usuário |
| UpdateUserRequest | Request para atualização de nome/email |
| ChangePasswordRequest | Request para troca de senha |

#### Views
- **ProfilePage**: Perfil com avatar (iniciais), nome, email, notificações, logout
- **ChangeNameModal**: Alterar nome
- **ChangeEmailModal**: Alterar email com confirmação de senha
- **ChangePasswordModal**: Alterar senha

---

### 4.5 Módulo de Autenticação (Auth)

#### Views
- **LoginPage**: Email/senha com login Google OAuth
- **RegisterPage**: Criação de conta

---

## 5. Arquitetura MVVM

### BaseViewModel
Classe base para todos os ViewModels com:
- `IsBusy`: Indica operação em andamento
- `Title`: Título da página/modal
- `OnPropertyChanged()`: Notificação de mudança de propriedade
- `SetProperty<T>()`: Define propriedade com verificação de igualdade
- `IsCacheExpired()`: Verifica expiração de cache (padrão 5 min)

### Padrões de Data Binding
| Padrão | Uso |
|--------|-----|
| Property Change Notification | `SetProperty(ref _field, value)` |
| Commands | `System.Windows.Input.Command` |
| Collection Binding | `ObservableCollection<T>` e `SafeObservableCollection<T>` |
| Query Properties | `[QueryProperty]` attribute |
| Weak References | DatePicker para evitar memory leaks |
| Eventos para Comunicação | `OnSaved`, `OnCancelled`, `FiltersApplied` |

### SafeObservableCollection
Wrapper seguro que captura `ObjectDisposedException` durante notificações - evita crashes quando collections sobrevivem aos subscribers.

### Mensagens Cross-View
| Mensagem | Propósito |
|----------|-----------|
| SelectTabMessage | Navegação entre tabs |
| GoBackTabMessage | Voltar para tab anterior |
| MealFoodChangedMessage | Indica alteração em foods da refeição |

---

## 6. Navegação

### Shell Navigation
```csharp
// Voltar
await Shell.Current.GoToAsync("..");

// Navegar com parâmetros
await Shell.Current.GoToAsync("ManageTransactionModal", new Dictionary<string, object>
{
    { "Transaction", transaction }
});

// Query properties
[QueryProperty(nameof(TaskId), "taskId")]
```

### Estrutura de Rotas (AppShell.xaml.cs)
- **Auth**: LoginPage, RegisterPage
- **Task**: taskdetail, tasklabels, ManageTaskItemModal, FilterTaskItemPopup
- **Financial**: CategoriesPage, TransactionListPage, ManageTransactionModal, FilterTransactionModal
- **Nutrition**: ManageMealModal, MealDetailPage, ManageLiquidModal, DiaryDetailPage, FoodSearchPage, DailyProgressPage, DiaryHistoryPage
- **Profile**: ChangeNameModal, ChangeEmailModal, ChangePasswordModal

---

## 7. Serviços

### Configuração de HTTP
- Base URL: `https://api.lifesync.tech`
- AuthDelegatingHandler: Adiciona JWT token em todas as requisições
- Android: Usa AndroidMessageHandler com bypass de certificado

### Serviços Registrados (Singleton)
| Serviço | Interface |
|---------|-----------|
| ApiService<> | IApiService<> |
| AuthService | IAuthService |
| UserSession | IUserSession |
| TaskItemService | - |
| TaskLabelService | - |
| TransactionService | - |
| CategoryService | - |
| NutritionService | - |
| UserProfileService | - |

### ViewModels (Singleton para persistência de estado)
- TaskItemsViewModel, TaskLabelViewModel
- FinancialViewModel, CategoriesViewModel, TransactionListViewModel
- NutritionViewModel, ManageMealViewModel, MealDetailViewModel, ManageLiquidViewModel
- DiaryDetailViewModel, FoodSearchViewModel, EditMealFoodViewModel, DailyProgressViewModel
- DiaryHistoryViewModel, CreateDiaryViewModel
- ProfileViewModel, ChangeNameViewModel, ChangeEmailViewModel, ChangePasswordViewModel

---

## 8. Controles Customizados

### CustomTabBar
Barra de navegação inferior com 5 tabs (Financeiro, Academico, Tarefas, Nutricao, Perfil). Suporta:
- Seleção visual com ícones branco/preto
- Background escuro quando selecionado
- Command para seleção de tab

### CircularProgressView
Gráfico circular para displaying progresso (calorias, hidratação):
- Progress: double (0.0-1.0)
- ProgressColor / TrackColor
- StrokeWidth ajustável
- Desenha arco do topo (12 o'clock) no sentido horário

---

## 9. Converters

### Gerais
| Converter | Função |
|-----------|--------|
| InvertedBoolConverter | Inverte boolean |
| HasItemsConverter | Verifica se coleção tem itens |
| PriorityToColorConverter | Priority → cores (Low=#3D8A5A, Medium=#D89575, High=#D08068, Urgent=#FF4444) |
| StatusToColorConverter | Status → cores (Pending=#9C9B99, InProgress=#D4A64A, Completed=#3D8A5A) |
| StatusToIconConverter | Status → ícones (○ ● ✓ ✕) |
| LabelColorToConverter | LabelColor → cor sólida |

### Financeiros
| Converter | Função |
|-----------|--------|
| MoneyToStringConverter | Money → "R$ X,XX" |
| TransactionTypeToColorConverter | Income=Green, Expense=Red |
| TransactionTypeToSignConverter | Income="+", Expense="-" |

### Nutricionais
| Converter | Função |
|-----------|--------|
| LiquidNameToIconConverter | Nome → ícone (café=coffe.svg, chá=tea.svg, default=water.svg) |

---

## 10. Modelos de Domínio

### Namespace: LifeSyncApp.Models.Financial

```
RecurrenceFrequency (Enum)
├── Daily, Weekly, Monthly, Yearly

Currency (Enum)
├── USD ($), EUR (€), BRL (R$)

TransactionType (Enum)
├── Income (Receita), Expense (Despesa)

PaymentMethod (Enum)
├── Cash, CreditCard, DebitCard, BankTransfer, Pix, Other

Money (Class)
├── Amount: int (centavos)
├── Currency: Currency
├── ToDecimal(), FromDecimal(), ToFormattedString()

CategoryExpense (Class)
├── CategoryId, CategoryName, Amount, Percentage, ProgressValue

TransactionGroup : ObservableCollection<TransactionDTO>
├── Date, DateDisplay, TransactionCountDisplay
```

### Namespace: LifeSyncApp.Models.TaskManager

```
Status (Enum)
├── Pending, InProgress, Completed, Cancelled
├── Friendly strings em português

Priority (Enum)
├── Low, Medium, High, Urgent

LabelColor (Enum)
├── Red, Green, Blue, Yellow, Purple, Orange, Pink, Brown, Gray

TaskItem : INotifyPropertyChanged
├── Id, CreatedAt, UpdatedAt, UserId
├── Title, Description (notify)
├── Status, Priority, DueDate (notify)
├── Labels: List<TaskLabel> (notify)

TaskLabel : INotifyPropertyChanged
├── Id, CreatedAt, UpdatedAt
├── Name, LabelColor, UserId

TaskGroup : ObservableCollection<TaskItem>
├── DueDate, DisplayDate (Hoje/Amanhã/Ontem/Atrasada)
├── IsOverdue, TaskCount, TaskCountText
```

---

## 11. UI/UX - Padrões

### Skeleton Loading
Todas as páginas de dados carregados têm UI skeleton com animação fade:
```csharp
// Fades entre 0.4 e 1.0 opacity com SinInOut easing
await SkeletonContainer.FadeTo(0.4, 800, Easing.SinInOut);
```

### Bottom Sheet Pattern
Modais usam design consistente:
- Overlay semi-transparente (#80000000)
- Cantos superiores arredondados (24,24,0,0)
- Indicador de drag
- Efeito de sombra
- Tap fora para dispensar

### DataTriggers
Usados extensivamente para estilização condicional:
```xml
<DataTrigger TargetType="Label" Binding="{Binding Status}" Value="Completed">
    <Setter Property="TextDecorations" Value="Strikethrough"/>
    <Setter Property="TextColor" Value="#9C9B99"/>
</DataTrigger>
```

### Compiled Bindings
Páginas usam `x:DataType="viewmodel:ViewModelType"` para bindings compilados com verificação em tempo de compilação.

---

## 12. Fluxo de Dados

### Autenticação
```
LoginPage → AuthService.LoginAsync() → JWT Token
                                      ↓
                              UserSession (singleton)
                                      ↓
App.xaml.cs → verifica IsAuthenticated → MainPage
```

### Carregamento de Dados (ex: NutritionPage)
```
OnAppearing → NutritionViewModel.LoadDataCommand
                          ↓
              Task.WhenAll para chamadas paralelas:
              - GetTodayDiaryAsync()
              - GetDailyProgressAsync()
              - GetMealsAsync()
              - GetLiquidsAsync()
                          ↓
              MainThread.InvokeOnMainThreadAsync() para batch UI update
```

### Cache Pattern
```csharp
if (IsCacheExpired(_lastRefresh, 5)) // 5 minutos
{
    // Recarrega dados
}
```

---

## 13. Recursos de Acessibilidade

### Cores de Status/Prioridade
| Elemento | Cor | Hex |
|----------|-----|-----|
| Low Priority | Verde | #3D8A5A |
| Medium Priority | Laranja | #D89575 |
| High Priority | Vermelho Laranja | #D08068 |
| Urgent Priority | Vermelho | #FF4444 |
| Pending Status | Cinza | #9C9B99 |
| InProgress Status | Amarelo | #D4A64A |
| Completed Status | Verde | #3D8A5A |
| Income | Verde | #00a63e |
| Expense | Vermelho | #e7000b |

---

## 14. Segurança

### Autenticação JWT
- Token de acesso incluído em todas as requisições via `AuthDelegatingHandler`
- Refresh token para renovação de sessão
- Validação de token com `System.IdentityModel.Tokens.Jwt`

### Validação de Input
- ViewModels validam campos antes de enviar para API
- `CanExecute` em commands impede execução com dados inválidos
- Confirmação de senha no registro e troca de senha

---

## 15. Localização

Toda a interface está em **Português (Brasil)**:
- Labels de status: "Pendente", "Em progresso", "Completado", "Cancelado"
- Datas: "Hoje", "Amanhã", "Ontem", "(Atrasada)"
- Formatação: "dd 'de' MMMM 'de' yyyy" (ex: "06 de April de 2026")
- Moeda: Padrão brasileiro (R$ X,XX)

---

## 16. Extensões e Mapping

### TaskLabelMapping
```csharp
TaskLabel.ToDTO()
TaskLabel.ToDTOList()
TaskLabel.ToCreateDTO()
TaskLabel.ToUpdateDTO()
```

### TaskItemMapping
```csharp
TaskItem.ToDTO()
TaskItem.ToDTOList()
TaskItem.ToCreateDTO()
TaskItem.ToUpdateDTO()
```

---

## 17. Notas de Implementação

1. **CommunityToolkit.Mvvm** usado para `WeakReferenceMessenger` e infraestrutura MVVM
2. **Singleton ViewModels** mantêm estado entre navegações de tab
3. **Transient Views** evitam problemas de estado
4. **Content Extraction Pattern** em MainPage previne `ObjectDisposedException`
5. **Parallel API Calls** com `Task.WhenAll` para performance
6. **Debouncing** em busca de alimentos (400ms)
7. **WeakReference** para DatePicker evitando memory leaks
