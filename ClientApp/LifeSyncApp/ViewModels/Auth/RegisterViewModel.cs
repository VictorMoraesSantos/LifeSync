using LifeSyncApp.DTOs.Auth;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.UserSession;

namespace LifeSyncApp.ViewModels.Auth
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IUserSession _userSession;

        public RegisterViewModel(IAuthService authService, IUserSession userSession)
        {
            _authService = authService;
            _userSession = userSession;
            Title = "Criar Conta";
        }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
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

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
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

        public async Task RegisterAsync()
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
                var request = new RegisterRequest
                {
                    FirstName = FirstName.Trim(),
                    LastName = LastName.Trim(),
                    Email = Email.Trim(),
                    Password = Password
                };

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

        public async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
