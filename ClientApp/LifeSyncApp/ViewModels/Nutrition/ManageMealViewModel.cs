using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.Services.Nutrition;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class ManageMealViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;

        private string _name = string.Empty;
        private string _description = string.Empty;
        private bool _isEditing;
        private int _mealId;
        private int _diaryId;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            private set
            {
                SetProperty(ref _isEditing, value);
                Title = value ? "Editar Refeição" : "Nova Refeição";
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageMealViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Nova Refeição";

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize(int diaryId, MealDTO? meal = null)
        {
            _diaryId = diaryId;
            IsEditing = meal != null;

            if (meal != null)
            {
                _mealId = meal.Id;
                Name = meal.Name;
                Description = meal.Description ?? string.Empty;
            }
            else
            {
                _mealId = 0;
                Name = string.Empty;
                Description = string.Empty;
            }
        }

        private async Task SaveAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(Name))
                return;

            IsBusy = true;
            try
            {
                bool success;
                if (IsEditing)
                {
                    var dto = new UpdateMealDTO(_mealId, Name, Description);
                    success = await _nutritionService.UpdateMealAsync(_mealId, dto);
                }
                else
                {
                    var dto = new CreateMealDTO(_diaryId, Name, Description);
                    success = await _nutritionService.CreateMealAsync(dto);
                }

                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível salvar a refeição.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving meal: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
