using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.Models.Nutrition.Meal;
using LifeSyncApp.Models.Nutrition.MealFood;
using LifeSyncApp.Helpers;
using LifeSyncApp.Services.Nutrition;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class MealDetailViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;

        [ObservableProperty]
        private bool _isLoadingMeal = true;

        private MealDTO? _meal;
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
                    OnPropertyChanged(nameof(TotalProtein));
                    OnPropertyChanged(nameof(TotalLipids));
                    OnPropertyChanged(nameof(TotalCarbs));
                    IsLoadingMeal = false;
                }
            }
        }

        public SafeObservableCollection<MealFoodDTO> Foods { get; } = new();

        public decimal TotalProtein => Foods.Sum(f => (f.Protein ?? 0) * f.Quantity / 100m);
        public decimal TotalLipids => Foods.Sum(f => (f.Lipids ?? 0) * f.Quantity / 100m);
        public decimal TotalCarbs => Foods.Sum(f => (f.Carbohydrates ?? 0) * f.Quantity / 100m);

        public event EventHandler? MealDeleted;

        public MealDetailViewModel(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        public async Task RefreshMealAsync()
        {
            if (_meal == null) return;
            var mealId = _meal.Id;
            try
            {
                IsBusy = true;
                _nutritionService.InvalidateAllCache();
                var updated = await _nutritionService.GetMealByIdAsync(mealId);
                if (updated != null)
                {
                    _meal = null;
                    Meal = updated;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MealDetailVM] RefreshMealAsync error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SyncFoodsFromMeal(MealDTO meal)
        {
            if (MainThread.IsMainThread)
            {
                Foods.Clear();
                foreach (var food in meal.MealFoods)
                    Foods.Add(food);
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Foods.Clear();
                    foreach (var food in meal.MealFoods)
                        Foods.Add(food);
                });
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task OpenEditMealModalAsync()
        {
            if (_meal == null) return;
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.ManageMealModal, new Dictionary<string, object>
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

        [RelayCommand]
        private async Task DeleteMealAsync()
        {
            if (_meal == null) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar", $"Deseja remover a refeição '{_meal.Name}'?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.DeleteMealAsync(_meal.Id);
                if (success)
                {
                    MealDeleted?.Invoke(this, EventArgs.Empty);
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Erro", error ?? "Não foi possível remover a refeição.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task OpenAddFoodAsync()
        {
            if (_meal == null) return;
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.FoodSearchPage, new Dictionary<string, object>
                {
                    { "MealId", _meal.Id }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task OpenEditFoodAsync(MealFoodDTO? f)
        {
            if (f == null || _meal == null) return;
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.EditMealFoodModal, new Dictionary<string, object>
                {
                    { "MealFood", f }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteFoodAsync(MealFoodDTO? f)
        {
            if (f == null || _meal == null) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar", $"Remover '{f.Name}' da refeição?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.RemoveFoodFromMealAsync(_meal.Id, f.Id);
                if (success)
                    await RefreshMealAsync();
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", error ?? "Não foi possível remover o alimento.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
