using LifeSyncApp.ViewModels.Auth;

namespace LifeSyncApp.Views.Auth;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;
    private bool _isPasswordVisible;
    private bool _isConfirmPasswordVisible;

    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await _viewModel.RegisterAsync();
    }

    private void OnTogglePasswordTapped(object sender, TappedEventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
        PasswordEyeIcon.Source = _isPasswordVisible ? "eye_off.png" : "eye_on.png";
    }

    private void OnToggleConfirmPasswordTapped(object sender, TappedEventArgs e)
    {
        _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
        ConfirmPasswordEntry.IsPassword = !_isConfirmPasswordVisible;
        ConfirmPasswordEyeIcon.Source = _isConfirmPasswordVisible ? "eye_off.png" : "eye_on.png";
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await _viewModel.GoToLoginAsync();
    }

    private async void OnLoginTapped(object sender, TappedEventArgs e)
    {
        await _viewModel.GoToLoginAsync();
    }
}
