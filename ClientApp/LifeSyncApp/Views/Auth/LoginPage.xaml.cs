using LifeSyncApp.ViewModels.Auth;

namespace LifeSyncApp.Views.Auth;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;
    private bool _isPasswordVisible;

    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        _viewModel.LoginCommand.Execute(null);
    }

    private void OnTogglePasswordTapped(object sender, TappedEventArgs e)
    {
        _isPasswordVisible = !_isPasswordVisible;
        PasswordEntry.IsPassword = !_isPasswordVisible;
        EyeIcon.Source = _isPasswordVisible ? "eye_off.png" : "eye_on.png";
    }

    private async void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert("Em breve", "Funcionalidade de recuperacao de senha sera implementada em breve.", "OK");
    }

    private async void OnGoogleLoginTapped(object sender, TappedEventArgs e)
    {
        _viewModel.GoogleLoginCommand.Execute(null);
    }

    private async void OnRegisterTapped(object sender, TappedEventArgs e)
    {
        _viewModel.GoToRegisterCommand.Execute(null);
    }
}
