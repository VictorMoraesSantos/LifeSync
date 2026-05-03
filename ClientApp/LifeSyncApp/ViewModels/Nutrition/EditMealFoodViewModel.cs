using LifeSyncApp.Constants;
using LifeSyncApp.DTOs.Nutrition.MealFood;
using LifeSyncApp.Services.Nutrition;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class EditMealFoodViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;

        private MealFoodDTO? _mealFood;
        private string _quantityText = string.Empty;

        public MealFoodDTO? MealFood
        {
            get => _mealFood;
            set
            {
                SetProperty(ref _mealFood, value);
                OnPropertyChanged(nameof(FoodName));
                OnPropertyChanged(nameof(CaloriesInfo));
                OnPropertyChanged(nameof(ProteinInfo));
                OnPropertyChanged(nameof(LipidsInfo));
                OnPropertyChanged(nameof(CarbsInfo));
                OnPropertyChanged(nameof(TotalEstimated));
            }
        }

        public string FoodName => _mealFood?.Name ?? string.Empty;
        public string CaloriesInfo => _mealFood != null ? $"{_mealFood.Calories} kcal / 100g" : string.Empty;
        public string ProteinInfo => _mealFood?.Protein != null ? $"Prot {_mealFood.Protein:F1}g / 100g" : string.Empty;
        public string LipidsInfo => _mealFood?.Lipids != null ? $"Lip {_mealFood.Lipids:F1}g / 100g" : string.Empty;
        public string CarbsInfo => _mealFood?.Carbohydrates != null ? $"Carb {_mealFood.Carbohydrates:F1}g / 100g" : string.Empty;

        public string QuantityText
        {
            get => _quantityText;
            set
            {
                SetProperty(ref _quantityText, value);
                OnPropertyChanged(nameof(TotalEstimated));
            }
        }

        public string TotalEstimated
        {
            get
            {
                if (_mealFood != null && int.TryParse(_quantityText, out var qty) && qty > 0)
                {
                    var total = qty * _mealFood.Calories / 100;
                    return $"{total} kcal";
                }
                return "0 kcal";
            }
        }

        public int MealId { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SwapFoodCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public EditMealFoodViewModel(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Editar Alimento";

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
            SwapFoodCommand = new Command(async () => await SwapFoodAsync());
        }

        public void Initialize(int mealId, MealFoodDTO? mealFood)
        {
            MealId = mealId;
            MealFood = mealFood;
            QuantityText = mealFood?.Quantity.ToString() ?? string.Empty;
        }

        private async Task SaveAsync()
        {
            if (IsBusy || _mealFood == null) return;

            if (!int.TryParse(_quantityText, out var qty) || qty <= 0)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe uma quantidade válida.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var dto = new UpdateMealFoodDTO(_mealFood.Id, _mealFood.FoodId, qty);
                var (success, error) = await _nutritionService.UpdateMealFoodAsync(_mealFood.Id, dto);
                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível salvar.", "OK");
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

        private async Task SwapFoodAsync()
        {
            if (_mealFood == null) return;
            // Navigate to food search to swap
            OnCancelled?.Invoke(this, EventArgs.Empty);
            await Shell.Current.GoToAsync(AppRoutes.FoodSearchPage, new Dictionary<string, object>
            {
                { "MealId", MealId }
            });
        }
    }
}
