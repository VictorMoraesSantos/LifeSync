using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial
{
    [QueryProperty(nameof(Transaction), "Transaction")]
    public partial class TransactionDetailModal : ContentPage
    {
        private readonly TransactionDetailViewModel _viewModel;

        public TransactionDTO? Transaction { get; set; }

        public TransactionDetailModal(TransactionDetailViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;

            _viewModel.OnClosed += async (s, e) => await Shell.Current.GoToAsync("..");

            _viewModel.OnEditRequested += async (s, transaction) =>
            {
                await Shell.Current.GoToAsync("..");
                // Navigate to ManageTransactionModal with transaction parameter
                await Shell.Current.GoToAsync("ManageTransactionModal", new Dictionary<string, object>
                {
                    { "Transaction", transaction }
                });
            };

            _viewModel.OnDeleted += async (s, transactionId) =>
            {
                // Modal already closes internally
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Transaction != null)
            {
                _viewModel.Initialize(Transaction);
            }
        }
    }
}
