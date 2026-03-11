using LifeSyncApp.DTOs.Profile;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.Profile;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Profile
{
    public class ChangeEmailViewModel : BaseViewModel
    {
        private readonly UserProfileService _userProfileService;
        private readonly IAuthService _authService;

        private string _newEmail = string.Empty;
        public string NewEmail
        {
            get => _newEmail;
            set
            {
                if (SetProperty(ref _newEmail, value))
                    ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        private string _currentPassword = string.Empty;
        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                if (SetProperty(ref _currentPassword, value))
                    ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        public string CurrentFirstName { get; set; } = string.Empty;
        public string CurrentLastName { get; set; } = string.Empty;

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ChangeEmailViewModel(UserProfileService userProfileService, IAuthService authService)
        {
            _userProfileService = userProfileService;
            _authService = authService;
            Title = "Alterar Email";
            SaveCommand = new Command(async () => await SaveAsync(), CanSave);
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize(string firstName, string lastName)
        {
            CurrentFirstName = firstName;
            CurrentLastName = lastName;
            NewEmail = string.Empty;
            CurrentPassword = string.Empty;
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(NewEmail) && !string.IsNullOrWhiteSpace(CurrentPassword);

        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var userId = await _authService.GetUserIdAsync();
                var dto = new UpdateUserRequest
                {
                    FirstName = CurrentFirstName,
                    LastName = CurrentLastName,
                    Email = NewEmail.Trim()
                };

                var (success, error) = await _userProfileService.UpdateUserAsync(userId, dto);

                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
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
    }
}
