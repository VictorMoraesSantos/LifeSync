using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.UserSession;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class CreateDiaryViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;
        private readonly IUserSession _userSession;

        private DateTime _selectedDate = DateTime.Today;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? OnCreated;
        public event EventHandler? OnCancelled;

        public CreateDiaryViewModel(NutritionService nutritionService, IUserSession userSession)
        {
            _nutritionService = nutritionService;
            _userSession = userSession;
            Title = "Novo Diário";

            CreateCommand = new Command(async () => await CreateAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize()
        {
            SelectedDate = DateTime.Today;
        }

        private async Task CreateAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var date = DateOnly.FromDateTime(SelectedDate);
                var dto = new CreateDiaryDTO(_userSession.UserId, date);
                var id = await _nutritionService.CreateDiaryAsync(dto);

                if (id.HasValue)
                    OnCreated?.Invoke(this, EventArgs.Empty);
                else
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível criar o diário. Verifique se já existe um diário para esta data.", "OK");
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
    }
}
