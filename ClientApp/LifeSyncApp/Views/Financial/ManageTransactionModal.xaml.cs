using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial;

public partial class ManageTransactionModal : ContentPage
{
    private readonly ManageTransactionViewModel _viewModel;

    public ManageTransactionModal(ManageTransactionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Subscribe to events
        _viewModel.OnSaved += async (sender, args) =>
        {
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
        await _viewModel.InitializeAsync();
    }
}
