using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Models.Nutrition.Diary;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.UserSession;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class CreateDiaryViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;
        private readonly IUserSession _userSession;

        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;

        public event EventHandler? OnCreated;
        public event EventHandler? OnCancelled;

        public CreateDiaryViewModel(INutritionService nutritionService, IUserSession userSession)
        {
            _nutritionService = nutritionService;
            _userSession = userSession;
            Title = "Novo Diário";
        }

        public void Initialize()
        {
            SelectedDate = DateTime.Today;
        }

        [RelayCommand]
        private async Task CreateAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var date = DateOnly.FromDateTime(SelectedDate);
                var dto = new CreateDiaryDTO(_userSession.UserId, date);
                var (id, error) = await _nutritionService.CreateDiaryAsync(dto);

                if (id.HasValue)
                    OnCreated?.Invoke(this, EventArgs.Empty);
                else
                    await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível criar o diário.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating diary: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", "Erro ao criar diário.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Cancel() => OnCancelled?.Invoke(this, EventArgs.Empty);
    }
}
