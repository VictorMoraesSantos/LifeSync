using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.Services.Financial;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial
{
    public class TransactionDetailViewModel : ViewModels.BaseViewModel
    {
        private readonly TransactionService _transactionService;
        private TransactionDTO? _transaction;

        public TransactionDTO? Transaction
        {
            get => _transaction;
            private set => SetProperty(ref _transaction, value);
        }

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

        public void Initialize(TransactionDTO transaction)
        {
            Transaction = transaction;
        }

        private void OnEdit()
        {
            if (Transaction != null)
            {
                OnEditRequested?.Invoke(this, Transaction);
            }
        }

        private async Task OnDeleteAsync()
        {
            if (Transaction == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar Exclusão",
                "Tem certeza que deseja excluir esta transação?",
                "Sim",
                "Não");

            if (!confirm) return;

            IsBusy = true;
            try
            {
                bool success = await _transactionService.DeleteTransactionAsync(Transaction.Id);
                if (success)
                {
                    OnDeleted?.Invoke(this, Transaction.Id);
                    OnClosed?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Erro",
                        "Não foi possível excluir a transação.",
                        "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting transaction: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert(
                    "Erro",
                    "Ocorreu um erro ao excluir a transação.",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
