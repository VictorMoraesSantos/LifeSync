using System.Windows.Input;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.Profile;

namespace LifeSyncApp.ViewModels.Profile
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly UserProfileService _userProfileService;

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

        public ProfileViewModel(IAuthService authService, UserProfileService userProfileService)
        {
            _authService = authService;
            _userProfileService = userProfileService;
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

        public void UpdateUserInfo(string name, string email)
        {
            UserName = name;
            UserEmail = email;
            UpdateInitials();
            _lastRefresh = DateTime.Now;
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
            try
            {
                var userId = await _authService.GetUserIdAsync();
                var user = await _userProfileService.GetUserAsync(userId);

                if (user != null)
                {
                    UserName = !string.IsNullOrEmpty(user.FullName) ? user.FullName : $"{user.FirstName} {user.LastName}".Trim();
                    UserEmail = user.Email;
                }
                else
                {
                    UserName = "Usuário";
                    UserEmail = string.Empty;
                }

                UpdateInitials();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar dados do perfil: {ex.Message}");
                UserName = "Usuário";
                UserInitials = "US";
            }
        }

        private void UpdateInitials()
        {
            if (!string.IsNullOrEmpty(UserName))
            {
                var parts = UserName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                UserInitials = parts.Length >= 2
                    ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
                    : UserName[..Math.Min(2, UserName.Length)].ToUpper();
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
