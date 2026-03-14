using LifeSyncApp.DTOs.Financial.RecurrenceSchedule;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial.Transaction
{
    public class TransactionDetailViewModel : BaseViewModel
    {
        private readonly TransactionService _transactionService;
        private TransactionDTO? _transaction;
        private RecurrenceScheduleDTO? _schedule;

        public TransactionDTO? Transaction
        {
            get => _transaction;
            private set => SetProperty(ref _transaction, value);
        }

        public RecurrenceScheduleDTO? Schedule
        {
            get => _schedule;
            private set
            {
                if (SetProperty(ref _schedule, value))
                    OnPropertyChanged(nameof(HasSchedule));
            }
        }

        public bool HasSchedule => _schedule != null;

        public string FrequencyDisplay => _schedule?.Frequency.ToDisplayString() ?? string.Empty;
        public string NextOccurrenceDisplay => _schedule?.NextOccurrence.ToString("dd/MM/yyyy") ?? string.Empty;
        public string ScheduleStatusDisplay => _schedule?.IsActive == true ? "Ativo" : "Inativo";
        public string OccurrencesDisplay => _schedule?.MaxOccurrences.HasValue == true
            ? $"{_schedule.OccurrencesGenerated}/{_schedule.MaxOccurrences}"
            : $"{_schedule?.OccurrencesGenerated ?? 0} geradas";
        public string EndDateDisplay => _schedule?.EndDate?.ToString("dd/MM/yyyy") ?? "Sem data final";

        public ICommand CloseCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public event EventHandler? OnClosed;
        public event EventHandler<TransactionDTO>? OnEditRequested;
        public event EventHandler<int>? OnDeleted;

        public TransactionDetailViewModel(TransactionService transactionService)
        {
            _transactionService = transactionService;
            Title = "Detalhes da Transação";

            CloseCommand = new Command(() => OnClosed?.Invoke(this, EventArgs.Empty));
            EditCommand = new Command(OnEdit);
            DeleteCommand = new Command(async () => await OnDeleteAsync());
        }

        public async Task InitializeAsync(TransactionDTO transaction)
        {
            Transaction = transaction;
            Schedule = null;

            if (transaction.IsRecurring)
            {
                var schedule = await _transactionService.GetScheduleByTransactionIdAsync(transaction.Id);
                Schedule = schedule;
                OnPropertyChanged(nameof(FrequencyDisplay));
                OnPropertyChanged(nameof(NextOccurrenceDisplay));
                OnPropertyChanged(nameof(ScheduleStatusDisplay));
                OnPropertyChanged(nameof(OccurrencesDisplay));
                OnPropertyChanged(nameof(EndDateDisplay));
            }
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
