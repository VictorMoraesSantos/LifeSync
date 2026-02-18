using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial;

public partial class FinancialPage : ContentPage
{
    private readonly FinancialViewModel _viewModel;

    public FinancialPage(FinancialViewModel viewModel)
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
