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
        private readonly TransactionService _transactionService;
        private readonly IUserSession _userSession;
        private TransactionFilterDTO _currentFilter = new();
        private bool _filterSetFromNavigation = false;

        public ObservableCollection<TransactionGroup> GroupedTransactions { get; } = new();

        public TransactionFilterDTO? Filter
        {
            set
            {
                if (value != null)
                {
                    _currentFilter = value;
                    _filterSetFromNavigation = true;
                    _ = ApplyFilterAsync();
                }
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand OpenManageTransactionModalCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenDetailCommand { get; }
        public ICommand OpenFilterCommand { get; }

        public TransactionListViewModel(TransactionService transactionService, IUserSession userSession)
        {
            _transactionService = transactionService;
            _userSession = userSession;
            Title = "Transações";

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            OpenManageTransactionModalCommand = new Command(async () => await OpenManageTransactionModalAsync());
            RefreshCommand = new Command(async () => await LoadTransactionsAsync());
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

        private async Task LoadTransactionsAsync()
        {
            await FetchAndGroupTransactionsAsync("Ocorreu um erro ao carreagar as transações.");
        }

        private async Task ApplyFilterAsync()
        {
            await FetchAndGroupTransactionsAsync("Ocorreu um erro ao aplicar o filtro.");
        }

        private async Task FetchAndGroupTransactionsAsync(string errorMsg)
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var filter = _currentFilter with { UserId = _userSession.UserId };
                var transactions = await _transactionService.SearchTransactionsAsync(filter, cts.Token);
                var groups = transactions
                    .OrderByDescending(t => t.TransactionDate)
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new TransactionGroup(g.Key, g))
                    .ToList();

                GroupedTransactions.ReplaceAll(groups);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", errorMsg, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenManageTransactionModalAsync()
        {
            await Shell.Current.GoToAsync("ManageTransactionModal");
        }

        private async Task OpenDetailAsync(TransactionDTO? transaction)
        {
            if (transaction == null) return;

            await Shell.Current.GoToAsync("TransactionDetailModal", new Dictionary<string, object>
            {
                { "Transaction", transaction }
            });
        }

        private async Task OpenFilterAsync()
        {
            await Shell.Current.GoToAsync("FilterTransactionModal", new Dictionary<string, object>
            {
                { "ExistingFilter", _currentFilter }
            });
        }
    }
}
