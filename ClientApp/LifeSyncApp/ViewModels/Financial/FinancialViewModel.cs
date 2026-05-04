using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Helpers;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.Services.UserSession;
using System.Globalization;

namespace LifeSyncApp.ViewModels.Financial
{
    public partial class FinancialViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly ICategoryService _categoryService;
        private readonly IUserSession _userSession;

        [ObservableProperty]
        private bool _isLoadingData;

        private DateTime? _lastDataRefresh;

        [ObservableProperty]
        private string _currentMonthLabel = string.Empty;

        [ObservableProperty]
        private decimal _balance;

        [ObservableProperty]
        private decimal _totalIncome;

        [ObservableProperty]
        private decimal _totalExpense;

        [ObservableProperty]
        private int _transactionCount;

        [ObservableProperty]
        private int _incomeCount;

        [ObservableProperty]
        private int _expenseCount;

        [ObservableProperty]
        private decimal _highestIncome;

        [ObservableProperty]
        private decimal _highestExpense;

        [ObservableProperty]
        private TransactionDTO? _highestIncomeTransaction;

        [ObservableProperty]
        private TransactionDTO? _highestExpenseTransaction;

        public SafeObservableCollection<TransactionDTO> RecentTransactions { get; } = new();
        public SafeObservableCollection<CategoryExpense> TopCategories { get; } = new();
        public SafeObservableCollection<CategoryDTO> Categories { get; } = new();

        public bool HasRecentTransactions => RecentTransactions.Count > 0;
        public bool HasNoRecentTransactions => RecentTransactions.Count == 0;

        public bool HasTopCategories => TopCategories.Count > 0;
        public bool HasNoTopCategories => TopCategories.Count == 0;

        public FinancialViewModel(ITransactionService transactionService, ICategoryService categoryService, IUserSession userSession)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _userSession = userSession;
            Title = "Financeiro";
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task LoadDataAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCacheExpired(_lastDataRefresh) && RecentTransactions.Any()) return;

            if (IsLoadingData) return;

            try
            {
                IsLoadingData = true;
                IsBusy = true;

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
                    IsBusy = false;
                });

                _lastDataRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FinancialVM] {ex.GetType().Name}: {ex.Message}");
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

        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadDataAsync(forceRefresh: true);
        }

        [RelayCommand]
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

        [RelayCommand]
        private async Task OpenDetailAsync(TransactionDTO? transaction)
        {
            if (transaction == null) return;

            await Shell.Current.GoToAsync(AppRoutes.TransactionDetailModal, new Dictionary<string, object>
            {
                { "Transaction", transaction }
            });
        }

        [RelayCommand]
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

        [RelayCommand]
        private async Task ViewAllTransactionsAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.TransactionListPage);
        }

        [RelayCommand]
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

        [RelayCommand]
        private async Task OpenHighestIncomeDetailAsync()
        {
            if (HighestIncomeTransaction == null) return;
            await OpenDetailAsync(HighestIncomeTransaction);
        }

        [RelayCommand]
        private async Task OpenHighestExpenseDetailAsync()
        {
            if (HighestExpenseTransaction == null) return;
            await OpenDetailAsync(HighestExpenseTransaction);
        }
    }
}
