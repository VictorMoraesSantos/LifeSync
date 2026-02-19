using LifeSyncApp.DTOs.Nutrition.MealFood;
using LifeSyncApp.Services.Nutrition;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class ManageMealFoodViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;

        private string _name = string.Empty;
        private string _quantityText = string.Empty;
        private string _caloriesPerUnitText = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string QuantityText
        {
            get => _quantityText;
            set
            {
                SetProperty(ref _quantityText, value);
                OnPropertyChanged(nameof(TotalCalories));
            }
        }

        public string CaloriesPerUnitText
        {
            get => _caloriesPerUnitText;
            set
            {
                SetProperty(ref _caloriesPerUnitText, value);
                OnPropertyChanged(nameof(TotalCalories));
            }
        }

        public int TotalCalories
        {
            get
            {
                if (int.TryParse(_quantityText, out var qty) && int.TryParse(_caloriesPerUnitText, out var cal))
                    return qty * cal;
                return 0;
            }
        }

        public int MealId { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageMealFoodViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Adicionar Alimento";

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize(int mealId)
        {
            MealId = mealId;
            Name = string.Empty;
            QuantityText = string.Empty;
            CaloriesPerUnitText = string.Empty;
        }

        private async Task SaveAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(Name))
                return;

            if (!int.TryParse(_quantityText, out var qty) || qty <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert("Atenção", "Informe uma quantidade válida em gramas.", "OK");
                return;
            }

            if (!int.TryParse(_caloriesPerUnitText, out var cal) || cal < 0)
            {
                await Application.Current!.MainPage!.DisplayAlert("Atenção", "Informe as calorias por grama (pode ser 0).", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var dto = new CreateMealFoodDTO(Name, qty, cal);
                var success = await _nutritionService.AddFoodToMealAsync(MealId, dto);
                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível adicionar o alimento.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving food: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
