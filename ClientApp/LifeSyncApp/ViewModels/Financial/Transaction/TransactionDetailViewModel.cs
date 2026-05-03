using LifeSyncApp.DTOs.Financial.RecurrenceSchedule;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial.Transaction
{
    public class TransactionDetailViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;
        private TransactionDTO? _transaction;

        public TransactionDTO? Transaction
        {
            get => _transaction;
            private set
            {
                if (SetProperty(ref _transaction, value))
                {
                    OnPropertyChanged(nameof(HasSchedule));
                    OnPropertyChanged(nameof(Schedule));
                    OnPropertyChanged(nameof(FrequencyDisplay));
                    OnPropertyChanged(nameof(NextOccurrenceDisplay));
                    OnPropertyChanged(nameof(OccurrencesDisplay));
                    OnPropertyChanged(nameof(EndDateDisplay));
                }
            }
        }

        public RecurrenceScheduleInfoDTO? Schedule => _transaction?.RecurrenceSchedule;

        public bool HasSchedule => _transaction?.RecurrenceSchedule != null;

        public string FrequencyDisplay => Schedule?.Frequency.ToDisplayString() ?? string.Empty;
        public string NextOccurrenceDisplay => Schedule?.NextOccurrence.ToString("dd/MM/yyyy") ?? string.Empty;
        public string OccurrencesDisplay => Schedule?.MaxOccurrences.HasValue == true
            ? $"{Schedule.OccurrencesGenerated} / {Schedule.MaxOccurrences}"
            : $"{Schedule?.OccurrencesGenerated ?? 0} geradas";
        public string EndDateDisplay => Schedule?.EndDate?.ToString("dd/MM/yyyy") ?? "Sem data final";

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public event EventHandler? OnClosed;
        public event EventHandler<TransactionDTO>? OnEditRequested;
        public event EventHandler<int>? OnDeleted;

        public TransactionDetailViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            Title = "Detalhes da Transação";

            CloseCommand = new Command(() => OnClosed?.Invoke(this, EventArgs.Empty));
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(async () => await OnDeleteAsync());
        }

        public void Initialize(TransactionDTO transaction)
        {
            Transaction = transaction;
        }

        private void OnEdit()
        {
            if (Transaction != null)
                OnEditRequested?.Invoke(this, Transaction);
        }

        private async Task OnDeleteAsync()
        {
            if (Transaction == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirmar Exclusão",
                "Tem certeza que deseja excluir esta transação?",
                "Sim",
                "Não");

            if (!confirm) return;

            IsBusy = true;

            try
            {
                var (success, error) = await _transactionService.DeleteTransactionAsync(Transaction.Id);
                if (success)
                {
                    OnDeleted?.Invoke(this, Transaction.Id);
                    OnClosed?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível excluir a transação.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
