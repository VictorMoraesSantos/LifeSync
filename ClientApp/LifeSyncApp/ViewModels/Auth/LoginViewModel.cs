using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.DTOs.Auth;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.UserSession;

namespace LifeSyncApp.ViewModels.Auth
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IUserSession _userSession;

        public LoginViewModel(IAuthService authService, IUserSession userSession)
        {
            _authService = authService;
            _userSession = userSession;
            Title = "Login";
        }

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Preencha todos os campos.";
                HasError = true;
                return;
            }

            if (IsBusy) return;
            IsBusy = true;
            HasError = false;

            try
            {
                var request = new LoginRequest(Email.Trim(), Password);

                await _authService.LoginAsync(request);
                await _userSession.InitializeAsync();

                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erro: {ex.GetType().Name} - {ex.Message}";
                HasError = true;
                System.Diagnostics.Debug.WriteLine($"Login error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoogleLoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            HasError = false;

            try
            {
                await _authService.GoogleLoginAsync();
                await _userSession.InitializeAsync();

                await Shell.Current.GoToAsync("//MainPage");
            }
            catch (TimeoutException ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
            catch (TaskCanceledException ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                System.Diagnostics.Debug.WriteLine($"Google Login Error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
