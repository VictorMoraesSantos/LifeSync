using LifeSyncApp.Constants;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Helpers;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial.Transaction
{
    [QueryProperty(nameof(Filter), "Filter")]
    public class TransactionListViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserSession _userSession;
        private TransactionFilterDTO _currentFilter = new();
        private bool _filterSetFromNavigation = false;
        private DateTime? _lastTransactionsRefresh;
        private TransactionFilterDTO? _cachedFilter;
        private bool _isLoadingData;

        public bool IsLoadingData
        {
            get => _isLoadingData;
            private set => SetProperty(ref _isLoadingData, value);
        }

        public ObservableCollection<TransactionGroup> GroupedTransactions { get; } = new();

        public TransactionFilterDTO? Filter
        {
            set
            {
                if (value != null)
                {
                    _currentFilter = value;
                    _filterSetFromNavigation = true;
                    InvalidateTransactionsCache();
                    _ = ApplyFilterAsync();
                }
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand OpenManageTransactionModalCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenDetailCommand { get; }
        public ICommand OpenFilterCommand { get; }

        public TransactionListViewModel(ITransactionService transactionService, IUserSession userSession)
        {
            _transactionService = transactionService;
            _userSession = userSession;
            Title = "Transações";

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            OpenManageTransactionModalCommand = new Command(async () => await OpenManageTransactionModalAsync());
            RefreshCommand = new Command(async () => await LoadTransactionsAsync(forceRefresh: true));
            OpenDetailCommand = new Command<TransactionDTO>(async (transaction) => await OpenDetailAsync(transaction));
            OpenFilterCommand = new Command(async () => await OpenFilterAsync());
        }

        public async Task InitializeAsync()
        {
            if (!_filterSetFromNavigation)
                _currentFilter = new TransactionFilterDTO();

            _filterSetFromNavigation = false;

            await LoadTransactionsAsync();
        }

        public void InvalidateTransactionsCache()
        {
            _lastTransactionsRefresh = null;
            _cachedFilter = null;
        }

        private bool IsFilterChanged()
        {
            if (_cachedFilter == null) return true;
            return _cachedFilter != _currentFilter;
        }

        private async Task LoadTransactionsAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCacheExpired(_lastTransactionsRefresh) && !IsFilterChanged() && GroupedTransactions.Any())
                return;

            await FetchAndGroupTransactionsAsync("Ocorreu um erro ao carregar as transações.");
        }

        private async Task ApplyFilterAsync()
        {
            await FetchAndGroupTransactionsAsync("Ocorreu um erro ao aplicar o filtro.");
        }

        private async Task FetchAndGroupTransactionsAsync(string errorMsg)
        {
            if (IsLoadingData) return;

            try
            {
                IsLoadingData = true;
                IsBusy = true;
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

                var filter = _currentFilter with { UserId = _userSession.UserId };
                var transactions = await _transactionService.SearchTransactionsAsync(filter, cts.Token);
                var groups = transactions
                    .OrderByDescending(t => t.TransactionDate)
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new TransactionGroup(g.Key, g))
                    .ToList();

                GroupedTransactions.ReplaceAll(groups);
                _lastTransactionsRefresh = DateTime.Now;
                _cachedFilter = _currentFilter;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TransactionListVM] Error: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", errorMsg, "OK");
            }
            finally
            {
                IsLoadingData = false;
                IsBusy = false;
            }
        }

        private async Task OpenManageTransactionModalAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.ManageTransactionModal);
        }

        private async Task OpenDetailAsync(TransactionDTO? transaction)
        {
            if (transaction == null) return;

            await Shell.Current.GoToAsync(AppRoutes.TransactionDetailModal, new Dictionary<string, object>
            {
                { "Transaction", transaction }
            });
        }

        private async Task OpenFilterAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.FilterTransactionModal, new Dictionary<string, object>
            {
                { "ExistingFilter", _currentFilter }
            });
        }
    }
}
