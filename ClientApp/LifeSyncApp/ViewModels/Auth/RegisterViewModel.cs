using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Models.Auth;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.UserSession;

namespace LifeSyncApp.ViewModels.Auth
{
    public partial class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IUserSession _userSession;

        public RegisterViewModel(IAuthService authService, IUserSession userSession)
        {
            _authService = authService;
            _userSession = userSession;
            Title = "Criar Conta";
        }

        [ObservableProperty]
        private string _firstName = string.Empty;

        [ObservableProperty]
        private string _lastName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        [RelayCommand]
        private async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Preencha todos os campos.";
                HasError = true;
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "As senhas nao coincidem.";
                HasError = true;
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "A senha deve ter no minimo 6 caracteres.";
                HasError = true;
                return;
            }

            if (IsBusy) return;
            IsBusy = true;
            HasError = false;

            try
            {
                var request = new RegisterRequest(FirstName.Trim(), LastName.Trim(), Email.Trim(), Password);

                await _authService.RegisterAsync(request);
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
                System.Diagnostics.Debug.WriteLine($"Register error: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
