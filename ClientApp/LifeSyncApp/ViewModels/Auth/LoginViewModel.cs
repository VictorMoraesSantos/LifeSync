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

        public async Task GoogleLoginAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            HasError = false;
            System.Diagnostics.Debug.WriteLine("[LoginVM] Google login started");

            try
            {
                await _authService.GoogleLoginAsync();
                await _userSession.InitializeAsync();

                await Shell.Current.GoToAsync("//MainPage");
                System.Diagnostics.Debug.WriteLine("[LoginVM] Google login completed successfully");
            }
            catch (TimeoutException ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                System.Diagnostics.Debug.WriteLine($"[LoginVM] Google login timeout: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
                System.Diagnostics.Debug.WriteLine($"[LoginVM] Google login canceled: {ex.Message}");
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
                System.Diagnostics.Debug.WriteLine("[LoginVM] Google login flow finalized (IsBusy=false)");
            }
        }

        public async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync("//RegisterPage");
        }
    }
}
