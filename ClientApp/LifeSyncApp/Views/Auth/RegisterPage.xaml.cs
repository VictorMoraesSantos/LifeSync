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

    private void OnRegisterClicked(object sender, EventArgs e)
    {
        _viewModel.RegisterCommand.Execute(null);
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

    private void OnBackTapped(object sender, TappedEventArgs e)
    {
        _viewModel.GoToLoginCommand.Execute(null);
    }

    private void OnLoginTapped(object sender, TappedEventArgs e)
    {
        _viewModel.GoToLoginCommand.Execute(null);
    }
}
