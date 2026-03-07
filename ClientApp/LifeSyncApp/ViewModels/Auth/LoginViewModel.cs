using LifeSyncApp.DTOs.Auth;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.UserSession;

namespace LifeSyncApp.ViewModels.Auth
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IUserSession _userSession;

        public LoginViewModel(IAuthService authService, IUserSession userSession)
        {
            _authService = authService;
            _userSession = userSession;
            Title = "Login";
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private bool _hasError;
        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public async Task LoginAsync()
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
                var request = new LoginRequest
                {
                    Email = Email.Trim(),
                    Password = Password
                };

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

        public async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
