using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.DTOs.Profile;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.Profile;

namespace LifeSyncApp.ViewModels.Profile
{
    public partial class ChangeNameViewModel : BaseViewModel
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _firstName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _lastName = string.Empty;

        public string CurrentEmail { get; set; } = string.Empty;
        public string FullName { get; private set; } = string.Empty;

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ChangeNameViewModel(IUserProfileService userProfileService, IAuthService authService)
        {
            _userProfileService = userProfileService;
            _authService = authService;
            Title = "Alterar Nome";
        }

        public void Initialize(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            CurrentEmail = email;
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName);

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var userId = await _authService.GetUserIdAsync();
                var dto = new UpdateUserRequest(FirstName.Trim(), LastName.Trim(), CurrentEmail);

                var (success, error) = await _userProfileService.UpdateUserAsync(userId, dto);

                if (success)
                {
                    FullName = $"{dto.FirstName} {dto.LastName}".Trim();
                    OnSaved?.Invoke(this, EventArgs.Empty);
                }
                else
                    await Shell.Current.DisplayAlert("Erro", error ?? "Erro ao atualizar nome.", "OK");
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
