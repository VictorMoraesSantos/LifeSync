using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial;

public partial class TransactionListPage : ContentPage
{
    private readonly TransactionListViewModel _viewModel;

    public TransactionListPage(TransactionListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync();
    }
}
