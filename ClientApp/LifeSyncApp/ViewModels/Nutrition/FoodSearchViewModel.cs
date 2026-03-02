using LifeSyncApp.DTOs.Nutrition.Food;
using LifeSyncApp.DTOs.Nutrition.MealFood;
using LifeSyncApp.Services.Nutrition;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class FoodSearchViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;
        private CancellationTokenSource? _searchCts;

        private string _searchText = string.Empty;
        private FoodDTO? _selectedFood;
        private bool _isBottomSheetVisible;
        private string _quantityText = string.Empty;
        private int _resultCount;

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

        public int ResultCount
        {
            get => _resultCount;
            private set => SetProperty(ref _resultCount, value);
        }

        public FoodDTO? SelectedFood
        {
            get => _selectedFood;
            private set
            {
                SetProperty(ref _selectedFood, value);
                OnPropertyChanged(nameof(SelectedFoodCaloriesText));
                OnPropertyChanged(nameof(SelectedFoodProtein));
                OnPropertyChanged(nameof(SelectedFoodLipids));
                OnPropertyChanged(nameof(SelectedFoodCarbs));
                OnPropertyChanged(nameof(TotalCalories));
            }
        }

        public bool IsBottomSheetVisible
        {
            get => _isBottomSheetVisible;
            private set => SetProperty(ref _isBottomSheetVisible, value);
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

        public string SelectedFoodCaloriesText =>
            _selectedFood != null ? $"{_selectedFood.Calories} kcal / 100g" : string.Empty;

        public string SelectedFoodProtein =>
            _selectedFood?.Protein != null ? $"{_selectedFood.Protein:0.#}g" : "0g";

        public string SelectedFoodLipids =>
            _selectedFood?.Lipids != null ? $"{_selectedFood.Lipids:0.#}g" : "0g";

        public string SelectedFoodCarbs =>
            _selectedFood?.Carbohydrates != null ? $"{_selectedFood.Carbohydrates:0.#}g" : "0g";

        public int TotalCalories
        {
            get
            {
                if (_selectedFood != null && int.TryParse(_quantityText, out var qty) && qty > 0)
                    return (int)Math.Round(_selectedFood.Calories * qty / 100.0);
                return 0;
            }
        }

        public int MealId { get; private set; }

        public ICommand SearchCommand { get; }
        public ICommand SelectFoodCommand { get; }
        public ICommand CloseBottomSheetCommand { get; }
        public ICommand AddFoodCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public FoodSearchViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Buscar Alimento";

            SearchCommand = new Command(async () => await SearchAsync());
            SelectFoodCommand = new Command<FoodDTO>(SelectFood);
            CloseBottomSheetCommand = new Command(CloseBottomSheet);
            AddFoodCommand = new Command(async () => await AddFoodAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
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

            _ = SearchAsync();
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
                // Debounce cancelled, expected behavior
            }
        }

        private async Task SearchAsync(CancellationToken ct = default)
        {
            if (IsBusy) return;

            IsBusy = true;
            try
            {
                var nameFilter = string.IsNullOrWhiteSpace(_searchText) ? null : _searchText.Trim();
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

        private void SelectFood(FoodDTO? food)
        {
            if (food == null) return;
            SelectedFood = food;
            QuantityText = "100";
            IsBottomSheetVisible = true;
        }

        private void CloseBottomSheet()
        {
            IsBottomSheetVisible = false;
            SelectedFood = null;
            QuantityText = string.Empty;
        }

        private async Task AddFoodAsync()
        {
            if (IsBusy || _selectedFood == null) return;

            if (!int.TryParse(_quantityText, out var qty) || qty <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert("Atenção", "Informe uma quantidade válida em gramas.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var dto = new CreateMealFoodDTO(MealId, _selectedFood.Id, qty);
                var success = await _nutritionService.AddFoodToMealAsync(MealId, dto);
                if (success)
                {
                    CloseBottomSheet();
                    OnSaved?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível adicionar o alimento.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FoodSearchVM] AddFood error: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Erro", "Ocorreu um erro inesperado.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
