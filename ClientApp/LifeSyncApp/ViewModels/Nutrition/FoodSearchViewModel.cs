using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Models.Nutrition.Food;
using LifeSyncApp.Models.Nutrition.MealFood;
using LifeSyncApp.Services.Nutrition;
using System.Collections.ObjectModel;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class FoodSearchViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;
        private CancellationTokenSource? _searchCts;

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    _ = DebouncedSearchAsync();
            }
        }

        public ObservableCollection<FoodDTO> SearchResults { get; } = new();

        [ObservableProperty]
        private int _resultCount;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SelectedFoodCaloriesText))]
        [NotifyPropertyChangedFor(nameof(SelectedFoodProtein))]
        [NotifyPropertyChangedFor(nameof(SelectedFoodLipids))]
        [NotifyPropertyChangedFor(nameof(SelectedFoodCarbs))]
        [NotifyPropertyChangedFor(nameof(TotalCalories))]
        private FoodDTO? _selectedFood;

        [ObservableProperty]
        private bool _isBottomSheetVisible;

        private string _quantityText = string.Empty;
        public string QuantityText
        {
            get => _quantityText;
            set
            {
                SetProperty(ref _quantityText, value);
                OnPropertyChanged(nameof(TotalCalories));
            }
        }

        public string SelectedFoodCaloriesText =>
            SelectedFood != null ? $"{SelectedFood.Calories} kcal / 100g" : string.Empty;

        public string SelectedFoodProtein =>
            SelectedFood?.Protein != null ? $"{SelectedFood.Protein:0.#}g" : "0g";

        public string SelectedFoodLipids =>
            SelectedFood?.Lipids != null ? $"{SelectedFood.Lipids:0.#}g" : "0g";

        public string SelectedFoodCarbs =>
            SelectedFood?.Carbohydrates != null ? $"{SelectedFood.Carbohydrates:0.#}g" : "0g";

        public int TotalCalories
        {
            get
            {
                if (SelectedFood != null && int.TryParse(_quantityText, out var qty) && qty > 0)
                    return (int)Math.Round(SelectedFood.Calories * qty / 100.0);
                return 0;
            }
        }

        public int MealId { get; private set; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public FoodSearchViewModel(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Buscar Alimento";
        }

        public void Initialize(int mealId)
        {
            MealId = mealId;
            SearchText = string.Empty;
            SearchResults.Clear();
            ResultCount = 0;
            SelectedFood = null;
            IsBottomSheetVisible = false;
            QuantityText = string.Empty;
        }

        private async Task DebouncedSearchAsync()
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            try
            {
                await Task.Delay(400, token);
                if (!token.IsCancellationRequested)
                    await SearchAsync(token);
            }
            catch (TaskCanceledException)
            {
            }
        }

        [RelayCommand]
        private async Task SearchAsync(CancellationToken ct = default)
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(_searchText))
            {
                SearchResults.Clear();
                ResultCount = 0;
                return;
            }

            IsBusy = true;
            try
            {
                var nameFilter = _searchText.Trim();
                var results = await _nutritionService.SearchFoodsAsync(nameFilter, page: 1, pageSize: 50, ct: ct);

                SearchResults.Clear();
                foreach (var food in results)
                    SearchResults.Add(food);

                ResultCount = SearchResults.Count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodSearchVM] Search error: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void SelectFood(FoodDTO? food)
        {
            if (food == null) return;
            SelectedFood = food;
            QuantityText = "100";
            IsBottomSheetVisible = true;
        }

        [RelayCommand]
        private void CloseBottomSheet()
        {
            IsBottomSheetVisible = false;
            SelectedFood = null;
            QuantityText = string.Empty;
        }

        [RelayCommand]
        private async Task AddFoodAsync()
        {
            if (IsBusy || SelectedFood == null)
                return;

            if (!int.TryParse(_quantityText, out var qty) || qty <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert("Atenção", "Informe uma quantidade válida em gramas.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var dto = new CreateMealFoodDTO(MealId, SelectedFood.Id, qty);
                var (success, error) = await _nutritionService.AddFoodToMealAsync(MealId, dto);

                if (success)
                {
                    CloseBottomSheet();
                    OnSaved?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Erro", error ?? "Não foi possível adicionar o alimento.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Erro inesperado: {ex.Message}", "OK");
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
