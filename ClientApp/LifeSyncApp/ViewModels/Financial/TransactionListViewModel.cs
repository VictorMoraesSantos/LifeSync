using System.Collections.ObjectModel;
using System.Windows.Input;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial
{
    public class TransactionGroup : ObservableCollection<TransactionDTO>
    {
        public DateTime Date { get; }
        public string DateDisplay { get; }
        public string TransactionCountDisplay { get; }

        public TransactionGroup(DateTime date, IEnumerable<TransactionDTO> transactions) : base(transactions)
        {
            Date = date;
            DateDisplay = date.ToString("dd 'de' MMMM 'de' yyyy", new System.Globalization.CultureInfo("pt-BR"));
            var count = this.Count;
            TransactionCountDisplay = count == 1 ? "1 transação" : $"{count} transações";
        }
    }

    [QueryProperty(nameof(Filter), "Filter")]
    public class TransactionListViewModel : ViewModels.BaseViewModel
    {
        private readonly TransactionService _transactionService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação
        private TransactionFilterDTO _currentFilter = new();

        public ObservableCollection<TransactionGroup> GroupedTransactions { get; } = new();

        public TransactionFilterDTO? Filter
        {
            set
            {
                if (value != null)
                {
                    _currentFilter = value;
                    _ = ApplyFilterAsync();
                }
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand OpenManageTransactionModalCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenDetailCommand { get; }
        public ICommand OpenFilterCommand { get; }

        public TransactionListViewModel(TransactionService transactionService)
        {
            _transactionService = transactionService;
            Title = "Transações";

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            OpenManageTransactionModalCommand = new Command(async () => await OpenManageTransactionModalAsync());
            RefreshCommand = new Command(async () => await LoadTransactionsAsync());
            OpenDetailCommand = new Command<TransactionDTO>(async (transaction) => await OpenDetailAsync(transaction));
            OpenFilterCommand = new Command(async () => await OpenFilterAsync());
        }

        public async Task InitializeAsync()
        {
            await LoadTransactionsAsync();
        }

        private async Task LoadTransactionsAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var filter = _currentFilter with { UserId = _userId };
                var transactions = await _transactionService.SearchTransactionsAsync(filter, cts.Token);

                var groups = transactions
                    .OrderByDescending(t => t.TransactionDate)
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new TransactionGroup(g.Key, g))
                    .ToList();

                GroupedTransactions.Clear();
                foreach (var group in groups)
                {
                    GroupedTransactions.Add(group);
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("API call timeout loading transactions");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading transactions: {ex.Message}");
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
            await Shell.Current.GoToAsync("FilterTransactionModal");
        }

        private async Task ApplyFilterAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var filter = _currentFilter with { UserId = _userId };
                var transactions = await _transactionService.SearchTransactionsAsync(filter, cts.Token);

                var groups = transactions
                    .OrderByDescending(t => t.TransactionDate)
                    .GroupBy(t => t.TransactionDate.Date)
                    .Select(g => new TransactionGroup(g.Key, g))
                    .ToList();

                GroupedTransactions.Clear();
                foreach (var group in groups)
                {
                    GroupedTransactions.Add(group);
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("API call timeout applying filter");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying filter: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
