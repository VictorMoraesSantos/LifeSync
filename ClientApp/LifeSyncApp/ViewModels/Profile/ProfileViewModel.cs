using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.Profile;

namespace LifeSyncApp.ViewModels.Profile
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly IUserProfileService _userProfileService;

        [ObservableProperty]
        private string _userName = string.Empty;

        [ObservableProperty]
        private string _userEmail = string.Empty;

        [ObservableProperty]
        private string _userInitials = string.Empty;

        [ObservableProperty]
        private bool _notificationsEnabled = true;

        [ObservableProperty]
        private bool _isLoadingData;

        private DateTime? _lastRefresh;

        public ProfileViewModel(IAuthService authService, IUserProfileService userProfileService)
        {
            _authService = authService;
            _userProfileService = userProfileService;
            Title = "Configurações";
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
                IsLoadingData = true;
                await LoadUserInfoAsync();
                _lastRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar perfil: {ex.Message}");
            }
            finally
            {
                IsLoadingData = false;
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

        [RelayCommand]
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

        [RelayCommand]
        private async Task ChangeNameAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.ChangeNameModal);
        }

        [RelayCommand]
        private async Task ChangeEmailAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.ChangeEmailModal);
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.ChangePasswordModal);
        }
    }
}
