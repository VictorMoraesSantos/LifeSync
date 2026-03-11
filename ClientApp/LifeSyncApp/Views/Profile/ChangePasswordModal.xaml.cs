using LifeSyncApp.ViewModels.Profile;

namespace LifeSyncApp.Views.Profile;

public partial class ChangePasswordModal : ContentPage
{
    private readonly ChangePasswordViewModel _viewModel;

    public ChangePasswordModal(ChangePasswordViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnSaved += OnSaved;
        _viewModel.OnCancelled += OnCancelled;
        _viewModel.Initialize();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        await Shell.Current.DisplayAlert("Sucesso", "Senha alterada com sucesso.", "OK");
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
