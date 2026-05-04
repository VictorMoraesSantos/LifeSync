using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Models.Profile;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.Profile;

namespace LifeSyncApp.ViewModels.Profile
{
    public partial class ChangeEmailViewModel : BaseViewModel
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _newEmail = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _currentPassword = string.Empty;

        public string CurrentFirstName { get; set; } = string.Empty;
        public string CurrentLastName { get; set; } = string.Empty;
        public string UpdatedEmail { get; private set; } = string.Empty;

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ChangeEmailViewModel(IUserProfileService userProfileService, IAuthService authService)
        {
            _userProfileService = userProfileService;
            _authService = authService;
            Title = "Alterar Email";
        }

        public void Initialize(string firstName, string lastName)
        {
            CurrentFirstName = firstName;
            CurrentLastName = lastName;
            NewEmail = string.Empty;
            CurrentPassword = string.Empty;
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(NewEmail) && !string.IsNullOrWhiteSpace(CurrentPassword);

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var userId = await _authService.GetUserIdAsync();
                var dto = new UpdateUserRequest(CurrentFirstName, CurrentLastName, NewEmail.Trim());

                var (success, error) = await _userProfileService.UpdateUserAsync(userId, dto);

                if (success)
                {
                    UpdatedEmail = dto.Email;
                    OnSaved?.Invoke(this, EventArgs.Empty);
                }
                else
                    await Shell.Current.DisplayAlert("Erro", error ?? "Erro ao atualizar email.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}
