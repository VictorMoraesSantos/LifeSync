using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial
{
    [QueryProperty(nameof(Transaction), "Transaction")]
    public partial class TransactionDetailModal : ContentPage
    {
        private readonly TransactionDetailViewModel _viewModel;
        private readonly FinancialViewModel _financialViewModel;

        public TransactionDTO? Transaction { get; set; }

        public TransactionDetailModal(TransactionDetailViewModel viewModel, FinancialViewModel financialViewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _financialViewModel = financialViewModel;
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _viewModel.OnClosed += OnClosed;
            _viewModel.OnEditRequested += OnEditRequested;
            _viewModel.OnDeleted += OnDeleted;

            if (Transaction != null)
                _viewModel.Initialize(Transaction);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _viewModel.OnClosed -= OnClosed;
            _viewModel.OnEditRequested -= OnEditRequested;
            _viewModel.OnDeleted -= OnDeleted;
        }

        private async void OnClosed(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private async void OnEditRequested(object? sender, TransactionDTO transaction)
        {
            await Shell.Current.GoToAsync("..");
            await Shell.Current.GoToAsync("ManageTransactionModal", new Dictionary<string, object>
            {
                { "Transaction", transaction }
            });
        }

        private async void OnDeleted(object? sender, int transactionId)
        {
            await _financialViewModel.LoadDataAsync(forceRefresh: true);
        }
    }
}
