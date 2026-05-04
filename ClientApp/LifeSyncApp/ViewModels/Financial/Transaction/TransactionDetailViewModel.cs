using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.DTOs.Financial.RecurrenceSchedule;
using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Models.Financial;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial.Transaction
{
    public partial class TransactionDetailViewModel : BaseViewModel
    {
        private readonly ITransactionService _transactionService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSchedule))]
        [NotifyPropertyChangedFor(nameof(Schedule))]
        [NotifyPropertyChangedFor(nameof(FrequencyDisplay))]
        [NotifyPropertyChangedFor(nameof(NextOccurrenceDisplay))]
        [NotifyPropertyChangedFor(nameof(OccurrencesDisplay))]
        [NotifyPropertyChangedFor(nameof(EndDateDisplay))]
        private TransactionDTO? _transaction;

        public RecurrenceScheduleInfoDTO? Schedule => Transaction?.RecurrenceSchedule;

        public bool HasSchedule => Transaction?.RecurrenceSchedule != null;

        public string FrequencyDisplay => Schedule?.Frequency.ToDisplayString() ?? string.Empty;
        public string NextOccurrenceDisplay => Schedule?.NextOccurrence.ToString("dd/MM/yyyy") ?? string.Empty;
        public string OccurrencesDisplay => Schedule?.MaxOccurrences.HasValue == true
            ? $"{Schedule.OccurrencesGenerated} / {Schedule.MaxOccurrences}"
            : $"{Schedule?.OccurrencesGenerated ?? 0} geradas";
        public string EndDateDisplay => Schedule?.EndDate?.ToString("dd/MM/yyyy") ?? "Sem data final";

        public event EventHandler? OnClosed;
        public event EventHandler<TransactionDTO>? OnEditRequested;
        public event EventHandler<int>? OnDeleted;

        public TransactionDetailViewModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
            Title = "Detalhes da Transação";
        }

        public void Initialize(TransactionDTO transaction)
        {
            Transaction = transaction;
        }

        [RelayCommand]
        private void Close()
        {
            OnClosed?.Invoke(this, EventArgs.Empty);
        }

        [RelayCommand]
        private void Edit()
        {
            if (Transaction != null)
                OnEditRequested?.Invoke(this, Transaction);
        }

        [RelayCommand]
        private async Task DeleteAsync()
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
