using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.Models.Nutrition.MealFood;
using LifeSyncApp.Services.Nutrition;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class EditMealFoodViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FoodName))]
        [NotifyPropertyChangedFor(nameof(CaloriesInfo))]
        [NotifyPropertyChangedFor(nameof(ProteinInfo))]
        [NotifyPropertyChangedFor(nameof(LipidsInfo))]
        [NotifyPropertyChangedFor(nameof(CarbsInfo))]
        [NotifyPropertyChangedFor(nameof(TotalEstimated))]
        private MealFoodDTO? _mealFood;

        public string FoodName => MealFood?.Name ?? string.Empty;
        public string CaloriesInfo => MealFood != null ? $"{MealFood.Calories} kcal / 100g" : string.Empty;
        public string ProteinInfo => MealFood?.Protein != null ? $"Prot {MealFood.Protein:F1}g / 100g" : string.Empty;
        public string LipidsInfo => MealFood?.Lipids != null ? $"Lip {MealFood.Lipids:F1}g / 100g" : string.Empty;
        public string CarbsInfo => MealFood?.Carbohydrates != null ? $"Carb {MealFood.Carbohydrates:F1}g / 100g" : string.Empty;

        private string _quantityText = string.Empty;
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
                if (MealFood != null && int.TryParse(_quantityText, out var qty) && qty > 0)
                {
                    var total = qty * MealFood.Calories / 100;
                    return $"{total} kcal";
                }
                return "0 kcal";
            }
        }

        public int MealId { get; private set; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public EditMealFoodViewModel(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Editar Alimento";
        }

        public void Initialize(int mealId, MealFoodDTO? mealFood)
        {
            MealId = mealId;
            MealFood = mealFood;
            QuantityText = mealFood?.Quantity.ToString() ?? string.Empty;
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy || MealFood == null) return;

            if (!int.TryParse(_quantityText, out var qty) || qty <= 0)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe uma quantidade válida.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var dto = new UpdateMealFoodDTO(MealFood.Id, MealFood.FoodId, qty);
                var (success, error) = await _nutritionService.UpdateMealFoodAsync(MealFood.Id, dto);
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

        [RelayCommand]
        private void Cancel() => OnCancelled?.Invoke(this, EventArgs.Empty);

        [RelayCommand]
        private async Task SwapFoodAsync()
        {
            if (MealFood == null) return;
            OnCancelled?.Invoke(this, EventArgs.Empty);
            await Shell.Current.GoToAsync(AppRoutes.FoodSearchPage, new Dictionary<string, object>
            {
                { "MealId", MealId }
            });
        }
    }
}
