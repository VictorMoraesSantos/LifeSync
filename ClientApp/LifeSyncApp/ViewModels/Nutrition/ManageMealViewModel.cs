using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.Services.Nutrition;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class ManageMealViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;

        private int _mealId;
        private int _diaryId;

        [ObservableProperty]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _description = string.Empty;

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            private set
            {
                SetProperty(ref _isEditing, value);
                Title = value ? "Editar Refeição" : "Nova Refeição";
            }
        }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageMealViewModel(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Nova Refeição";
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

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(Name))
                return;

            IsBusy = true;
            try
            {
                bool success;
                string? error;

                if (IsEditing)
                {
                    var dto = new UpdateMealDTO(_mealId, Name, Description);
                    (success, error) = await _nutritionService.UpdateMealAsync(_mealId, dto);
                }
                else
                {
                    var dto = new CreateMealDTO(_diaryId, Name, Description);
                    (success, error) = await _nutritionService.CreateMealAsync(dto);
                }

                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", error ?? "Não foi possível salvar a refeição.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
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
