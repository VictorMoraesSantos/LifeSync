using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Models.Profile;
using LifeSyncApp.Services.Profile;

namespace LifeSyncApp.ViewModels.Profile
{
    public partial class ChangePasswordViewModel : BaseViewModel
    {
        private readonly IUserProfileService _userProfileService;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _currentPassword = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _newPassword = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _confirmPassword = string.Empty;

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ChangePasswordViewModel(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
            Title = "Alterar Senha";
        }

        public void Initialize()
        {
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        private bool CanSave() =>
            !string.IsNullOrWhiteSpace(CurrentPassword) &&
            !string.IsNullOrWhiteSpace(NewPassword) &&
            !string.IsNullOrWhiteSpace(ConfirmPassword);

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (IsBusy) return;

            if (NewPassword != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Erro", "As senhas não coincidem.", "OK");
                return;
            }

            IsBusy = true;

            try
            {
                var dto = new ChangePasswordRequest(CurrentPassword, NewPassword);

                var (success, error) = await _userProfileService.ChangePasswordAsync(dto);

                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Shell.Current.DisplayAlert("Erro", error ?? "Erro ao alterar senha.", "OK");
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
