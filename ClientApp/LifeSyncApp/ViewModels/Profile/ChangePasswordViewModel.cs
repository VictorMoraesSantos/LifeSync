using LifeSyncApp.DTOs.Profile;
using LifeSyncApp.Services.Profile;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Profile
{
    public class ChangePasswordViewModel : BaseViewModel
    {
        private readonly IUserProfileService _userProfileService;

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

        private string _newPassword = string.Empty;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetProperty(ref _newPassword, value))
                    ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                if (SetProperty(ref _confirmPassword, value))
                    ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ChangePasswordViewModel(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
            Title = "Alterar Senha";
            SaveCommand = new Command(async () => await SaveAsync(), CanSave);
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
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
    }
}
