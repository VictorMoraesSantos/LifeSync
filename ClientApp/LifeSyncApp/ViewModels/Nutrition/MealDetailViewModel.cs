using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.DTOs.Nutrition.MealFood;
using LifeSyncApp.Services.Nutrition;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class MealDetailViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;

        private MealDTO? _meal;
        private int _diaryId;

        public MealDTO? Meal
        {
            get => _meal;
            set
            {
                SetProperty(ref _meal, value);
                if (value != null)
                {
                    SyncFoodsFromMeal(value);
                    Title = value.Name;
                }
            }
        }

        public ObservableCollection<MealFoodDTO> Foods { get; } = new();

        public ICommand GoBackCommand { get; }
        public ICommand OpenEditMealModalCommand { get; }
        public ICommand DeleteMealCommand { get; }
        public ICommand OpenAddFoodModalCommand { get; }
        public ICommand DeleteFoodCommand { get; }

        public event EventHandler? MealDeleted;

        public MealDetailViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            OpenEditMealModalCommand = new Command(async () => await OpenEditMealModalAsync());
            DeleteMealCommand = new Command(async () => await DeleteMealAsync());
            OpenAddFoodModalCommand = new Command(async () => await OpenAddFoodModalAsync());
            DeleteFoodCommand = new Command<MealFoodDTO>(async (f) => await DeleteFoodAsync(f));
        }

        public async Task RefreshMealAsync()
        {
            if (_meal == null) return;
            try
            {
                IsBusy = true;
                var updated = await _nutritionService.GetMealByIdAsync(_meal.Id);
                if (updated != null)
                    Meal = updated;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing meal: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SyncFoodsFromMeal(MealDTO meal)
        {
            Foods.Clear();
            foreach (var food in meal.MealFoods)
                Foods.Add(food);
        }

        private async Task OpenEditMealModalAsync()
        {
            if (_meal == null) return;
            try
            {
                await Shell.Current.GoToAsync("ManageMealModal", new Dictionary<string, object>
                {
                    { "DiaryId", _meal.DiaryId },
                    { "Meal", _meal }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async Task DeleteMealAsync()
        {
            if (_meal == null) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar", $"Deseja remover a refeição '{_meal.Name}'?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var success = await _nutritionService.DeleteMealAsync(_meal.Id);
                if (success)
                {
                    MealDeleted?.Invoke(this, EventArgs.Empty);
                    await Shell.Current.GoToAsync("..");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenAddFoodModalAsync()
        {
            if (_meal == null) return;
            try
            {
                await Shell.Current.GoToAsync("ManageMealFoodModal", new Dictionary<string, object>
                {
                    { "MealId", _meal.Id }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async Task DeleteFoodAsync(MealFoodDTO? food)
        {
            if (food == null || _meal == null) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar", $"Remover '{food.Name}' da refeição?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var success = await _nutritionService.RemoveFoodFromMealAsync(_meal.Id, food.Id);
                if (success)
                    await RefreshMealAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
