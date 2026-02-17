using LifeSyncApp.DTOs.Financial.Transaction;
using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial;

[QueryProperty(nameof(Transaction), "Transaction")]
public partial class ManageTransactionModal : ContentPage
{
    private readonly ManageTransactionViewModel _viewModel;

    public TransactionDTO? Transaction { get; set; }

    public ManageTransactionModal(ManageTransactionViewModel viewModel, FinancialViewModel financialViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.OnSaved += async (sender, args) =>
        {
            financialViewModel.InvalidateDataCache();
            await Shell.Current.GoToAsync("..");
        };

        _viewModel.OnCancelled += async (sender, args) =>
        {
            await Shell.Current.GoToAsync("..");
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync(Transaction);
    }
}
