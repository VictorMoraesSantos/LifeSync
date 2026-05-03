using LifeSyncApp.Constants;
using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Helpers;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial
{
    public class FinancialViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly IUserSession _userSession;

        private bool _isLoadingData;
        public bool IsLoadingData
        {
            get => _isLoadingData;
            private set => SetProperty(ref _isLoadingData, value);
        }
        private DateTime? _lastDataRefresh;

        private string _currentMonthLabel = string.Empty;
        public string CurrentMonthLabel
        {
            get => _currentMonthLabel;
            set => SetProperty(ref _currentMonthLabel, value);
        }

        private decimal _balance;
        private decimal _totalIncome;
        private decimal _totalExpense;
        private int _transactionCount;
        private int _incomeCount;
        private int _expenseCount;
        private decimal _highestIncome;
        private decimal _highestExpense;
        private TransactionDTO? _highestIncomeTransaction;
        private TransactionDTO? _highestExpenseTransaction;

        public decimal Balance
        {
            get => _balance;
            set => SetProperty(ref _balance, value);
        }

        public decimal TotalIncome
        {
            get => _totalIncome;
            set => SetProperty(ref _totalIncome, value);
        }

        public decimal TotalExpense
        {
            get => _totalExpense;
            set => SetProperty(ref _totalExpense, value);
        }

        public int TransactionCount
        {
            get => _transactionCount;
            set => SetProperty(ref _transactionCount, value);
        }

        public int IncomeCount
        {
            get => _incomeCount;
            set => SetProperty(ref _incomeCount, value);
        }

        public int ExpenseCount
        {
            get => _expenseCount;
            set => SetProperty(ref _expenseCount, value);
        }

        public decimal HighestIncome
        {
            get => _highestIncome;
            set => SetProperty(ref _highestIncome, value);
        }

        public decimal HighestExpense
        {
            get => _highestExpense;
            set => SetProperty(ref _highestExpense, value);
        }

        public TransactionDTO? HighestIncomeTransaction
        {
            get => _highestIncomeTransaction;
            private set => SetProperty(ref _highestIncomeTransaction, value);
        }

        public TransactionDTO? HighestExpenseTransaction
        {
            get => _highestExpenseTransaction;
            private set => SetProperty(ref _highestExpenseTransaction, value);
        }

        public SafeObservableCollection<TransactionDTO> RecentTransactions { get; } = new();
        public SafeObservableCollection<CategoryExpense> TopCategories { get; } = new();
        public SafeObservableCollection<CategoryDTO> Categories { get; } = new();

        public bool HasRecentTransactions => RecentTransactions.Count > 0;
        public bool HasNoRecentTransactions => RecentTransactions.Count == 0;

        public bool HasTopCategories => TopCategories.Count > 0;
        public bool HasNoTopCategories => TopCategories.Count == 0;

        public ICommand LoadDataCommand { get; }
        public ICommand OpenManageTransactionModalCommand { get; }
        public ICommand OpenDetailCommand { get; }
        public ICommand GoToCategoriesCommand { get; }
        public ICommand ViewAllTransactionsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenCategoryTransactionsCommand { get; }
        public ICommand OpenHighestIncomeDetailCommand { get; }
        public ICommand OpenHighestExpenseDetailCommand { get; }

        public FinancialViewModel(ITransactionService transactionService, ICategoryService categoryService, IUserSession userSession)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _userSession = userSession;
            Title = "Financeiro";

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            OpenManageTransactionModalCommand = new Command(async () => await OpenManageTransactionModalAsync());
            OpenDetailCommand = new Command<TransactionDTO>(async (t) => await OpenDetailAsync(t));
            GoToCategoriesCommand = new Command(async () => await GoToCategoriesAsync());
            ViewAllTransactionsCommand = new Command(async () => await ViewAllTransactionsAsync());
            RefreshCommand = new Command(async () => await LoadDataAsync(forceRefresh: true));
            OpenCategoryTransactionsCommand = new Command<CategoryExpense>(async (cat) => await OpenCategoryTransactionsAsync(cat));
            OpenHighestIncomeDetailCommand = new Command(async () => await OpenHighestIncomeDetailAsync());
            OpenHighestExpenseDetailCommand = new Command(async () => await OpenHighestExpenseDetailAsync());
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        public async Task LoadDataAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCacheExpired(_lastDataRefresh) && RecentTransactions.Any()) return;

            if (IsLoadingData) return;

            try
            {
                IsLoadingData = true;
                IsBusy = true;

                // Fetch categories and transactions in parallel, off the main thread
                var categoriesTask = _categoryService.GetCategoriesByUserIdAsync(_userSession.UserId);

                var now = DateTime.Now;
                var monthName = now.ToString("MMMM", new CultureInfo("pt-BR"));
                CurrentMonthLabel = $"Saldo de {monthName}";

                var startOfMonth = new DateOnly(now.Year, now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                var filter = new TransactionFilterDTO(UserId: _userSession.UserId, TransactionDateFrom: startOfMonth, TransactionDateTo: endOfMonth);
                var transactionsTask = _transactionService.SearchTransactionsAsync(filter);

                await Task.WhenAll(categoriesTask, transactionsTask).ConfigureAwait(false);

                var categories = categoriesTask.Result;
                var transactions = transactionsTask.Result;

                // Calculate statistics off main thread
                var incomes = transactions.Where(t => t.TransactionType == TransactionType.Income).ToList();
                var expenses = transactions.Where(t => t.TransactionType == TransactionType.Expense).ToList();

                var totalIncome = incomes.Sum(t => t.Amount?.ToDecimal() ?? 0m);
                var totalExpense = expenses.Sum(t => t.Amount?.ToDecimal() ?? 0m);
                var balance = totalIncome - totalExpense;
                var transactionCount = transactions.Count;
                var incomeCount = incomes.Count;
                var expenseCount = expenses.Count;
                var highestIncome = incomes.Any() ? incomes.Max(t => t.Amount?.ToDecimal() ?? 0m) : 0;
                var highestExpense = expenses.Any() ? expenses.Max(t => t.Amount?.ToDecimal() ?? 0m) : 0;
                var highestIncomeTransaction = incomes.FirstOrDefault(t => (t.Amount?.ToDecimal() ?? 0m) == highestIncome);
                var highestExpenseTransaction = expenses.FirstOrDefault(t => (t.Amount?.ToDecimal() ?? 0m) == highestExpense);

                var recentTransactions = transactions.OrderByDescending(t => t.TransactionDate).Take(5).ToList();

                System.Diagnostics.Debug.WriteLine($"[FinancialVM] Total transactions: {transactions.Count}, Expenses: {expenses.Count}");
                System.Diagnostics.Debug.WriteLine($"[FinancialVM] Expenses with Category != null: {expenses.Count(t => t.Category != null)}");
                foreach (var exp in expenses.Take(3))
                    System.Diagnostics.Debug.WriteLine($"[FinancialVM] Expense sample: Id={exp.Id}, Category={exp.Category?.Name ?? "NULL"}, Amount={exp.Amount?.ToDecimal()}");

                var topCategories = expenses
                    .Where(t => t.Category != null)
                    .GroupBy(t => t.Category!.Id)
                    .Select(g => new CategoryExpense
                    {
                        CategoryId = g.Key,
                        CategoryName = g.First().Category!.Name,
                        Amount = g.Sum(t => t.Amount?.ToDecimal() ?? 0m),
                        Percentage = totalExpense > 0 ? (double)((g.Sum(t => t.Amount?.ToDecimal() ?? 0m) / totalExpense) * 100) : 0
                    })
                    .OrderByDescending(c => c.Amount)
                    .Take(5)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"[FinancialVM] TopCategories count: {topCategories.Count}");
                foreach (var cat in topCategories)
                    System.Diagnostics.Debug.WriteLine($"[FinancialVM] TopCategory: {cat.CategoryName} = R${cat.Amount:N2} ({cat.Percentage:F1}%)");

                // Batch all UI updates on main thread
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Categories.ReplaceAll(categories);
                    TotalIncome = totalIncome;
                    TotalExpense = totalExpense;
                    Balance = balance;
                    TransactionCount = transactionCount;
                    IncomeCount = incomeCount;
                    ExpenseCount = expenseCount;
                    HighestIncome = highestIncome;
                    HighestExpense = highestExpense;
                    HighestIncomeTransaction = highestIncomeTransaction;
                    HighestExpenseTransaction = highestExpenseTransaction;
                    RecentTransactions.ReplaceAll(recentTransactions);
                    TopCategories.ReplaceAll(topCategories);
                    OnPropertyChanged(nameof(HasRecentTransactions));
                    OnPropertyChanged(nameof(HasNoRecentTransactions));
                    OnPropertyChanged(nameof(HasTopCategories));
                    OnPropertyChanged(nameof(HasNoTopCategories));
                    System.Diagnostics.Debug.WriteLine($"[FinancialVM] UI updated - TopCategories.Count: {TopCategories.Count}, HasTopCategories: {HasTopCategories}");
                    IsBusy = false;
                });

                _lastDataRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FinancialVM] {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}");
                if (ex.InnerException != null)
                    System.Diagnostics.Debug.WriteLine($"[FinancialVM] Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    IsBusy = false;
                    await Shell.Current.DisplayAlert("Erro", $"Não foi possível carregar os dados financeiros.\n[{ex.GetType().Name}] {ex.Message}", "OK");
                });
            }
            finally
            {
                IsLoadingData = false;
            }
        }

        public void InvalidateDataCache()
        {
            _lastDataRefresh = null;
        }

        private void CalculateStatistics(List<TransactionDTO> transactions)
        {
            var incomes = transactions.Where(t => t.TransactionType == TransactionType.Income).ToList();
            var expenses = transactions.Where(t => t.TransactionType == TransactionType.Expense).ToList();

            TotalIncome = incomes.Sum(t => t.Amount?.ToDecimal() ?? 0m);
            TotalExpense = expenses.Sum(t => t.Amount?.ToDecimal() ?? 0m);
            Balance = TotalIncome - TotalExpense;

            TransactionCount = transactions.Count;
            IncomeCount = incomes.Count;
            ExpenseCount = expenses.Count;

            HighestIncome = incomes.Any() ? incomes.Max(t => t.Amount?.ToDecimal() ?? 0m) : 0;
            HighestExpense = expenses.Any() ? expenses.Max(t => t.Amount?.ToDecimal() ?? 0m) : 0;
        }

        private void CalculateTopCategories(List<TransactionDTO> transactions)
        {
            var expenses = transactions.Where(t => t.TransactionType == TransactionType.Expense && t.Category != null);
            var totalExpenses = TotalExpense;
            var categoryGroups = expenses
                .GroupBy(t => t.Category!.Id)
                .Select(g => new CategoryExpense
                {
                    CategoryId = g.Key,
                    CategoryName = g.First().Category!.Name,
                    Amount = g.Sum(t => t.Amount?.ToDecimal() ?? 0m),
                    Percentage = totalExpenses > 0 ? (double)((g.Sum(t => t.Amount?.ToDecimal() ?? 0m) / totalExpenses) * 100) : 0
                })
                .OrderByDescending(c => c.Amount)
                .Take(5);

            TopCategories.ReplaceAll(categoryGroups);
        }

        private async Task OpenManageTransactionModalAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.ManageTransactionModal);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível abrir o modal: {ex.Message}", "OK");
            }
        }

        private async Task GoToCategoriesAsync()
        {
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.CategoriesPage);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível abrir categorias: {ex.Message}", "OK");
            }
        }

        private async Task ViewAllTransactionsAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.TransactionListPage);
        }

        private async Task OpenDetailAsync(TransactionDTO? transaction)
        {
            if (transaction == null) return;

            await Shell.Current.GoToAsync(AppRoutes.TransactionDetailModal, new Dictionary<string, object>
            {
                { "Transaction", transaction }
            });
        }

        private async Task OpenCategoryTransactionsAsync(CategoryExpense? category)
        {
            if (category == null) return;

            var today = DateTime.Today;
            var startOfMonth = new DateOnly(today.Year, today.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var filter = new TransactionFilterDTO(
                UserId: _userSession.UserId,
                CategoryId: category.CategoryId,
                TransactionType: TransactionType.Expense,
                TransactionDateFrom: startOfMonth,
                TransactionDateTo: endOfMonth
            );

            await Shell.Current.GoToAsync(AppRoutes.TransactionListPage, new Dictionary<string, object>
            {
                { "Filter", filter }
            });
        }

        private async Task OpenHighestIncomeDetailAsync()
        {
            if (HighestIncomeTransaction == null) return;
            await OpenDetailAsync(HighestIncomeTransaction);
        }

        private async Task OpenHighestExpenseDetailAsync()
        {
            if (HighestExpenseTransaction == null) return;
            await OpenDetailAsync(HighestExpenseTransaction);
        }
    }
}
