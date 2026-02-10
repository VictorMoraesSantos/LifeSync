using System.Collections.ObjectModel;
using System.Windows.Input;
using LifeSyncApp.DTOs.Financial;
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

    public class TransactionListViewModel : ViewModels.BaseViewModel
    {
        private readonly TransactionService _transactionService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        public ObservableCollection<TransactionGroup> GroupedTransactions { get; } = new();

        public ICommand GoBackCommand { get; }
        public ICommand OpenManageTransactionModalCommand { get; }
        public ICommand RefreshCommand { get; }

        public TransactionListViewModel(TransactionService transactionService)
        {
            _transactionService = transactionService;
            Title = "Transações";

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            OpenManageTransactionModalCommand = new Command(async () => await OpenManageTransactionModalAsync());
            RefreshCommand = new Command(async () => await LoadTransactionsAsync());
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

                var filter = new TransactionFilterDTO
                {
                    UserId = _userId
                };

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
    }
}
