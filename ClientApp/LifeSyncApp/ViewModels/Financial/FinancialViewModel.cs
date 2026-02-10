using System.Collections.ObjectModel;
using System.Windows.Input;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial
{
    public class FinancialViewModel : ViewModels.BaseViewModel
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

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
        public ICommand GoToCategoriesCommand { get; }
        public ICommand ViewAllTransactionsCommand { get; }
        public ICommand RefreshCommand { get; }

        public FinancialViewModel(TransactionService transactionService, CategoryService categoryService)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            Title = "Financeiro";

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            OpenManageTransactionModalCommand = new Command(async () => await OpenManageTransactionModalAsync());
            GoToCategoriesCommand = new Command(async () => await GoToCategoriesAsync());
            ViewAllTransactionsCommand = new Command(async () => await ViewAllTransactionsAsync());
            RefreshCommand = new Command(async () => await RefreshAsync());

            // DADOS MOCKADOS PARA TESTES
            LoadMockData();

            System.Diagnostics.Debug.WriteLine("FinancialViewModel initialized");
        }

        private void LoadMockData()
        {
            System.Diagnostics.Debug.WriteLine("Loading mock data for testing");

            Balance = 1500.50m;
            TotalIncome = 5000m;
            TotalExpense = 3499.50m;
            TransactionCount = 12;
            IncomeCount = 3;
            ExpenseCount = 9;
            HighestIncome = 2500m;
            HighestExpense = 850m;

            System.Diagnostics.Debug.WriteLine($"Mock data loaded: Balance={Balance}, Income={TotalIncome}, Expense={TotalExpense}");
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {

                System.Diagnostics.Debug.WriteLine("Loading financial data...");

                // Carregar categorias com timeout
                var categories = await _categoryService.GetCategoriesByUserIdAsync(_userId);
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
                System.Diagnostics.Debug.WriteLine($"Loaded {categories.Count} categories");

                // Carregar transações do mês atual
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var filter = new TransactionFilterDTO
                {
                    UserId = _userId,
                    TransactionDateFrom = startOfMonth,
                    TransactionDateTo = endOfMonth
                };

                var transactions = await _transactionService.SearchTransactionsAsync(filter);
                System.Diagnostics.Debug.WriteLine($"Loaded {transactions.Count} transactions");

                // Calcular estatísticas
                CalculateStatistics(transactions);

                // Carregar transações recentes (últimas 5)
                RecentTransactions.Clear();
                foreach (var transaction in transactions.OrderByDescending(t => t.TransactionDate).Take(5))
                {
                    RecentTransactions.Add(transaction);
                }

                // Calcular top categorias
                CalculateTopCategories(transactions);

                System.Diagnostics.Debug.WriteLine("Financial data loaded successfully");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("API call timeout - using mock data");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
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
                    CategoryName = g.First().Category!.Name,
                    Amount = g.Sum(t => t.Amount.ToDecimal()),
                    Percentage = totalExpenses > 0 ? (double)((g.Sum(t => t.Amount.ToDecimal()) / totalExpenses) * 100) : 0
                })
                .OrderByDescending(c => c.Amount)
                .Take(5);

            TopCategories.Clear();
            foreach (var category in categoryGroups)
            {
                TopCategories.Add(category);
            }
        }

        private async Task OpenManageTransactionModalAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Opening ManageTransactionModal...");
                await Shell.Current.GoToAsync("ManageTransactionModal");
                System.Diagnostics.Debug.WriteLine("ManageTransactionModal navigation completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening transaction modal: {ex.Message}\n{ex.StackTrace}");
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível abrir o modal: {ex.Message}", "OK");
            }
        }

        private async Task GoToCategoriesAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Opening CategoriesPage...");
                await Shell.Current.GoToAsync("CategoriesPage");
                System.Diagnostics.Debug.WriteLine("CategoriesPage navigation completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening categories: {ex.Message}\n{ex.StackTrace}");
                await Application.Current.MainPage.DisplayAlert("Erro", $"Não foi possível abrir categorias: {ex.Message}", "OK");
            }
        }

        private async Task ViewAllTransactionsAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("TransactionListPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening transaction list: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private async Task RefreshAsync()
        {
            await LoadDataAsync();
        }
    }

    public class CategoryExpense
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public double Percentage { get; set; }
    }
}
