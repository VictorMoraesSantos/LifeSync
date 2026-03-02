using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.ViewModels.Financial;
using LifeSyncApp.ViewModels.Financial.Transaction;

namespace LifeSyncApp.Views.Financial;

[QueryProperty(nameof(Transaction), "Transaction")]
public partial class ManageTransactionModal : ContentPage
{
    private readonly ManageTransactionViewModel _viewModel;
    private readonly FinancialViewModel _financialViewModel;
    private readonly TransactionListViewModel _transactionListViewModel;

    public TransactionDTO? Transaction { get; set; }

    public ManageTransactionModal(ManageTransactionViewModel viewModel, FinancialViewModel financialViewModel, TransactionListViewModel transactionListViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _financialViewModel = financialViewModel;
        _transactionListViewModel = transactionListViewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.OnSaved += OnSaved;
        _viewModel.OnCancelled += OnCancelled;

        await _viewModel.InitializeAsync(Transaction);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        _financialViewModel.InvalidateDataCache();
        _transactionListViewModel.InvalidateTransactionsCache();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
