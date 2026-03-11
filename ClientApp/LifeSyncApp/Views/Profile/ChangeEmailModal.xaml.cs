using LifeSyncApp.ViewModels.Profile;

namespace LifeSyncApp.Views.Profile;

public partial class ChangeEmailModal : ContentPage
{
    private readonly ChangeEmailViewModel _viewModel;
    private readonly ProfileViewModel _profileViewModel;

    public ChangeEmailModal(ChangeEmailViewModel viewModel, ProfileViewModel profileViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _profileViewModel = profileViewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnSaved += OnSaved;
        _viewModel.OnCancelled += OnCancelled;

        var nameParts = (_profileViewModel.UserName ?? "").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var firstName = nameParts.Length > 0 ? nameParts[0] : "";
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";
        _viewModel.Initialize(firstName, lastName);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        _profileViewModel.UpdateUserInfo(_profileViewModel.UserName, _viewModel.UpdatedEmail);
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
