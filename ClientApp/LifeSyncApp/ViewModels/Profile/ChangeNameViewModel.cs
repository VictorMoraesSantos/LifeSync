using LifeSyncApp.DTOs.Profile;
using LifeSyncApp.Services.Auth;
using LifeSyncApp.Services.Profile;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Profile
{
    public class ChangeNameViewModel : BaseViewModel
    {
        private readonly UserProfileService _userProfileService;
        private readonly IAuthService _authService;

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (SetProperty(ref _firstName, value))
                    ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set
            {
                if (SetProperty(ref _lastName, value))
                    ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        public string CurrentEmail { get; set; } = string.Empty;

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ChangeNameViewModel(UserProfileService userProfileService, IAuthService authService)
        {
            _userProfileService = userProfileService;
            _authService = authService;
            Title = "Alterar Nome";
            SaveCommand = new Command(async () => await SaveAsync(), CanSave);
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            CurrentEmail = email;
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName);

        private async Task SaveAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var userId = await _authService.GetUserIdAsync();
                var dto = new UpdateUserRequest
                {
                    FirstName = FirstName.Trim(),
                    LastName = LastName.Trim(),
                    Email = CurrentEmail
                };

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

        public string FullName { get; private set; } = string.Empty;
    }
}
