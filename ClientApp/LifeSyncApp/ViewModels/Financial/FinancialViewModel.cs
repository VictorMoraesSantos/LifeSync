using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial
{
    public class FinancialViewModel : BaseViewModel
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        // Cache management
        private bool _isLoadingData;
        private DateTime? _lastDataRefresh;
        private const int CacheExpirationMinutes = 5;

        private decimal _balance;
        private decimal _totalIncome;
        private decimal _totalExpense;
        private int _transactionCount;
        private int _incomeCount;
        private int _expenseCount;
        private decimal _highestIncome;
        private decimal _highestExpense;

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

        public ObservableCollection<TransactionDTO> RecentTransactions { get; } = new();
        public ObservableCollection<CategoryExpense> TopCategories { get; } = new();
        public ObservableCollection<CategoryDTO> Categories { get; } = new();

        public ICommand LoadDataCommand { get; }
        public ICommand OpenManageTransactionModalCommand { get; }
        public ICommand OpenDetailCommand { get; }
        public ICommand GoToCategoriesCommand { get; }
        public ICommand ViewAllTransactionsCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenCategoryTransactionsCommand { get; }

        public FinancialViewModel(TransactionService transactionService, CategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            Title = "Financeiro";

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            OpenManageTransactionModalCommand = new Command(async () => await OpenManageTransactionModalAsync());
            OpenDetailCommand = new Command<TransactionDTO>(async (t) => await OpenDetailAsync(t));
            GoToCategoriesCommand = new Command(async () => await GoToCategoriesAsync());
            ViewAllTransactionsCommand = new Command(async () => await ViewAllTransactionsAsync());
            RefreshCommand = new Command(async () => await LoadDataAsync(forceRefresh: true));
            OpenCategoryTransactionsCommand = new Command<CategoryExpense>(async (cat) => await OpenCategoryTransactionsAsync(cat));
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        public async Task LoadDataAsync(bool forceRefresh = false)
        {
            // Use cached data if still valid
            if (!forceRefresh && !IsDataCacheExpired() && RecentTransactions.Any())
            {
                System.Diagnostics.Debug.WriteLine("📦 Using cached financial data (not expired)");
                return;
            }

            // Prevent concurrent loads
            if (_isLoadingData)
            {
                System.Diagnostics.Debug.WriteLine("⏳ Financial data already loading, skipping duplicate request");
                return;
            }

            try
            {
                _isLoadingData = true;
                IsBusy = true;

                System.Diagnostics.Debug.WriteLine($"🔄 Loading financial data from API (forceRefresh: {forceRefresh})");

                var categories = await _categoryService.GetCategoriesByUserIdAsync(_userId);
                Categories.Clear();
                foreach (var category in categories)
                    Categories.Add(category);

                var startOfMonth = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                var filter = new TransactionFilterDTO(UserId: _userId, TransactionDateFrom: startOfMonth, TransactionDateTo: endOfMonth);
                var transactions = await _transactionService.SearchTransactionsAsync(filter);

                CalculateStatistics(transactions);

                RecentTransactions.Clear();
                foreach (var transaction in transactions.OrderByDescending(t => t.TransactionDate).Take(5))
                    RecentTransactions.Add(transaction);

                CalculateTopCategories(transactions);

                _lastDataRefresh = DateTime.Now;
                System.Diagnostics.Debug.WriteLine("✅ Financial data loaded successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading financial data: {ex.Message}");
            }
            finally
            {
                _isLoadingData = false;
                IsBusy = false;
            }
        }

        private bool IsDataCacheExpired()
        {
            if (_lastDataRefresh == null)
                return true;

            return (DateTime.Now - _lastDataRefresh.Value).TotalMinutes >= CacheExpirationMinutes;
        }

        public void InvalidateDataCache()
        {
            _lastDataRefresh = null;
            System.Diagnostics.Debug.WriteLine("🗑️ Financial data cache invalidated");
        }

        private void CalculateStatistics(List<TransactionDTO> transactions)
        {
            var incomes = transactions.Where(t => t.TransactionType == TransactionType.Income).ToList();
            var expenses = transactions.Where(t => t.TransactionType == TransactionType.Expense).ToList();

            TotalIncome = incomes.Sum(t => t.Amount.ToDecimal());
            TotalExpense = expenses.Sum(t => t.Amount.ToDecimal());
            Balance = TotalIncome - TotalExpense;

            TransactionCount = transactions.Count;
            IncomeCount = incomes.Count;
            ExpenseCount = expenses.Count;

            HighestIncome = incomes.Any() ? incomes.Max(t => t.Amount.ToDecimal()) : 0;
            HighestExpense = expenses.Any() ? expenses.Max(t => t.Amount.ToDecimal()) : 0;
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
                    Amount = g.Sum(t => t.Amount.ToDecimal()),
                    Percentage = totalExpenses > 0 ? (double)((g.Sum(t => t.Amount.ToDecimal()) / totalExpenses) * 100) : 0
                })
                .OrderByDescending(c => c.Amount)
                .Take(5);

            TopCategories.Clear();
            foreach (var category in categoryGroups)
                TopCategories.Add(category);
        }

        private async Task OpenManageTransactionModalAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("ManageTransactionModal");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível abrir o modal: {ex.Message}", "OK");
            }
        }

        private async Task GoToCategoriesAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("CategoriesPage");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível abrir categorias: {ex.Message}", "OK");
            }
        }

        private async Task ViewAllTransactionsAsync()
        {
            await Shell.Current.GoToAsync("TransactionListPage");
        }

        private async Task OpenDetailAsync(TransactionDTO? transaction)
        {
            if (transaction == null) return;

            await Shell.Current.GoToAsync("TransactionDetailModal", new Dictionary<string, object>
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
                UserId: _userId,
                CategoryId: category.CategoryId,
                TransactionType: TransactionType.Expense,
                TransactionDateFrom: startOfMonth,
                TransactionDateTo: endOfMonth
            );

            await Shell.Current.GoToAsync("TransactionListPage", new Dictionary<string, object>
            {
                { "Filter", filter }
            });
        }
    }

    public class CategoryExpense
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public double Percentage { get; set; }  // 0–100 for display label
        public double ProgressValue => Percentage / 100.0;  // 0–1 for ProgressBar
    }
}
