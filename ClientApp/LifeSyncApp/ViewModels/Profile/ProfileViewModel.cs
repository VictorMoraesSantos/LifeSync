using System.IdentityModel.Tokens.Jwt;
using System.Windows.Input;
using LifeSyncApp.Services.Auth;

namespace LifeSyncApp.ViewModels.Profile
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;

        private string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        private string _userEmail = string.Empty;
        public string UserEmail
        {
            get => _userEmail;
            set => SetProperty(ref _userEmail, value);
        }

        private string _userInitials = string.Empty;
        public string UserInitials
        {
            get => _userInitials;
            set => SetProperty(ref _userInitials, value);
        }

        private bool _notificationsEnabled = true;
        public bool NotificationsEnabled
        {
            get => _notificationsEnabled;
            set => SetProperty(ref _notificationsEnabled, value);
        }

        public ICommand LogoutCommand { get; }
        public ICommand ChangeNameCommand { get; }
        public ICommand ChangeEmailCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        private DateTime? _lastRefresh;

        public ProfileViewModel(IAuthService authService)
        {
            _authService = authService;
            Title = "Configurações";
            LogoutCommand = new Command(async () => await LogoutAsync());
            ChangeNameCommand = new Command(async () => await Shell.Current.GoToAsync("ChangeNameModal"));
            ChangeEmailCommand = new Command(async () => await Shell.Current.GoToAsync("ChangeEmailModal"));
            ChangePasswordCommand = new Command(async () => await Shell.Current.GoToAsync("ChangePasswordModal"));
        }

        public void InvalidateCache()
        {
            _lastRefresh = null;
        }

        public async Task InitializeAsync()
        {
            if (IsBusy) return;
            if (!IsCacheExpired(_lastRefresh)) return;

            try
            {
                IsBusy = true;
                await LoadUserInfoAsync();
                _lastRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar perfil: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadUserInfoAsync()
        {
            var token = await _authService.GetAccessTokenAsync();
            if (string.IsNullOrEmpty(token)) return;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                var nameClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == "name" || c.Type == "unique_name" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

                var emailClaim = jwtToken.Claims.FirstOrDefault(c =>
                    c.Type == "email" ||
                    c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");

                UserName = nameClaim?.Value ?? "Usuário";
                UserEmail = emailClaim?.Value ?? string.Empty;

                if (!string.IsNullOrEmpty(UserName))
                {
                    var parts = UserName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    UserInitials = parts.Length >= 2
                        ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
                        : UserName[..Math.Min(2, UserName.Length)].ToUpper();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao ler token JWT: {ex.Message}");
                UserName = "Usuário";
                UserInitials = "US";
            }
        }

        private async Task LogoutAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao fazer logout: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
