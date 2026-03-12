using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.Helpers;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.UserSession;
using System.Globalization;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class NutritionViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;
        private readonly IUserSession _userSession;
        private static readonly CultureInfo PtBr = new("pt-BR");

        private Task? _loadingTask;
        private DateTime? _lastDataRefresh;
        private DateOnly _selectedDate;

        private bool _isLoadingData;
        public bool IsLoadingData
        {
            get => _isLoadingData;
            private set => SetProperty(ref _isLoadingData, value);
        }

        private DiaryDTO? _todayDiary;
        private DailyProgressDTO? _dailyProgress;
        private int _caloriesConsumed;
        private int _caloriesGoal = 2000;
        private int _liquidsConsumedMl;
        private int _liquidsGoalMl = 2500;
        private string _dateLabel = string.Empty;
        private int _mealsCount;
        private int _liquidsCount;

        public DiaryDTO? TodayDiary
        {
            get => _todayDiary;
            set => SetProperty(ref _todayDiary, value);
        }

        public DailyProgressDTO? DailyProgress
        {
            get => _dailyProgress;
            set => SetProperty(ref _dailyProgress, value);
        }

        public DateOnly SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    UpdateDateLabel();
                    _ = LoadDataAsync(forceRefresh: true);
                }
            }
        }

        public string DateLabel
        {
            get => _dateLabel;
            set => SetProperty(ref _dateLabel, value);
        }

        public int CaloriesConsumed
        {
            get => _caloriesConsumed;
            set
            {
                SetProperty(ref _caloriesConsumed, value);
                OnPropertyChanged(nameof(CaloriesPercentage));
                OnPropertyChanged(nameof(CaloriesProgressValue));
                OnPropertyChanged(nameof(CaloriesDisplay));
            }
        }

        public int CaloriesGoal
        {
            get => _caloriesGoal;
            set
            {
                SetProperty(ref _caloriesGoal, value);
                OnPropertyChanged(nameof(CaloriesPercentage));
                OnPropertyChanged(nameof(CaloriesProgressValue));
                OnPropertyChanged(nameof(CaloriesDisplay));
                OnPropertyChanged(nameof(CaloriesGoalDisplay));
            }
        }

        public int CaloriesPercentage => CaloriesGoal > 0
            ? Math.Min((int)Math.Round(CaloriesConsumed * 100.0 / CaloriesGoal), 100)
            : 0;

        public double CaloriesProgressValue => CaloriesGoal > 0
            ? Math.Min(CaloriesConsumed / (double)CaloriesGoal, 1.0)
            : 0.0;

        public string CaloriesDisplay => $"{CaloriesConsumed:N0}";
        public string CaloriesGoalDisplay => $"de {CaloriesGoal:N0} kcal";

        public int LiquidsConsumedMl
        {
            get => _liquidsConsumedMl;
            set
            {
                SetProperty(ref _liquidsConsumedMl, value);
                OnPropertyChanged(nameof(LiquidsPercentage));
                OnPropertyChanged(nameof(LiquidsProgressValue));
                OnPropertyChanged(nameof(LiquidsDisplay));
            }
        }

        public int LiquidsGoalMl
        {
            get => _liquidsGoalMl;
            set
            {
                SetProperty(ref _liquidsGoalMl, value);
                OnPropertyChanged(nameof(LiquidsPercentage));
                OnPropertyChanged(nameof(LiquidsProgressValue));
                OnPropertyChanged(nameof(LiquidsDisplay));
                OnPropertyChanged(nameof(LiquidsGoalDisplay));
            }
        }

        public int LiquidsPercentage => LiquidsGoalMl > 0
            ? Math.Min((int)Math.Round(LiquidsConsumedMl * 100.0 / LiquidsGoalMl), 100)
            : 0;

        public double LiquidsProgressValue => LiquidsGoalMl > 0
            ? Math.Min(LiquidsConsumedMl / (double)LiquidsGoalMl, 1.0)
            : 0.0;

        public string LiquidsDisplay => $"{LiquidsConsumedMl:N0}";
        public string LiquidsGoalDisplay => $"de {LiquidsGoalMl:N0} ml";

        public int MealsCount
        {
            get => _mealsCount;
            set => SetProperty(ref _mealsCount, value);
        }

        public int LiquidsCount
        {
            get => _liquidsCount;
            set => SetProperty(ref _liquidsCount, value);
        }

        private decimal _totalProtein;
        private decimal _totalCarbs;
        private decimal _totalLipids;
        private decimal _totalSodium;

        public decimal TotalProtein
        {
            get => _totalProtein;
            set => SetProperty(ref _totalProtein, value);
        }

        public decimal TotalCarbs
        {
            get => _totalCarbs;
            set => SetProperty(ref _totalCarbs, value);
        }

        public decimal TotalLipids
        {
            get => _totalLipids;
            set => SetProperty(ref _totalLipids, value);
        }

        public decimal TotalSodium
        {
            get => _totalSodium;
            set => SetProperty(ref _totalSodium, value);
        }

        public string TotalProteinDisplay => $"{TotalProtein:0.##}g";
        public string TotalCarbsDisplay => $"{TotalCarbs:0.##}g";
        public string TotalLipidsDisplay => $"{TotalLipids:0.##}g";
        public string TotalSodiumDisplay => $"{TotalSodium / 1000m:0.##}g";

        public SafeObservableCollection<MealDTO> Meals { get; } = new();
        public SafeObservableCollection<LiquidDTO> Liquids { get; } = new();

        public ICommand LoadDataCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand PreviousDayCommand { get; }
        public ICommand NextDayCommand { get; }
        public ICommand OpenManageMealModalCommand { get; }
        public ICommand OpenManageLiquidModalCommand { get; }
        public ICommand OpenMealDetailCommand { get; }
        public ICommand OpenDiaryDetailCommand { get; }
        public ICommand OpenDailyProgressCommand { get; }
        public ICommand OpenDiaryHistoryCommand { get; }
        public ICommand EditLiquidCommand { get; }
        public ICommand DeleteLiquidCommand { get; }

        public NutritionViewModel(NutritionService nutritionService, IUserSession userSession)
        {
            _nutritionService = nutritionService;
            _userSession = userSession;
            Title = "Nutrição";
            _selectedDate = DateOnly.FromDateTime(DateTime.Today);
            UpdateDateLabel();

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            RefreshCommand = new Command(async () => await LoadDataAsync(forceRefresh: true));
            PreviousDayCommand = new Command(() => SelectedDate = SelectedDate.AddDays(-1));
            NextDayCommand = new Command(() => SelectedDate = SelectedDate.AddDays(1));
            OpenManageMealModalCommand = new Command(async () => await OpenManageMealModalAsync());
            OpenManageLiquidModalCommand = new Command(async () => await OpenManageLiquidModalAsync());
            OpenMealDetailCommand = new Command<MealDTO>(async (m) => await OpenMealDetailAsync(m));
            OpenDiaryDetailCommand = new Command(async () => await OpenDiaryDetailAsync());
            OpenDailyProgressCommand = new Command(async () => await OpenDailyProgressAsync());
            OpenDiaryHistoryCommand = new Command(async () => await Shell.Current.GoToAsync("DiaryHistoryPage"));
            EditLiquidCommand = new Command<LiquidDTO>(async (l) => await EditLiquidAsync(l));
            DeleteLiquidCommand = new Command<LiquidDTO>(async (l) => await DeleteLiquidAsync(l));
        }

        private void UpdateDateLabel()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (_selectedDate == today)
                DateLabel = $"Hoje, {_selectedDate.ToString("dd MMM", PtBr)}";
            else
                DateLabel = _selectedDate.ToString("dd 'de' MMMM", PtBr);
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        public Task LoadDataAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCacheExpired(_lastDataRefresh) && (Meals.Any()))
                return Task.CompletedTask;

            if (!forceRefresh && _loadingTask != null && !_loadingTask.IsCompleted)
                return _loadingTask;

            if (forceRefresh)
                _nutritionService.InvalidateAllCache();

            _loadingTask = LoadDataInternalAsync();
            return _loadingTask;
        }

        private async Task LoadDataInternalAsync()
        {
            try
            {
                IsLoadingData = true;
                IsBusy = true;

                var date = _selectedDate;

                // Fetch diaries and progresses in parallel (off main thread for deserialization)
                var diariesTask = _nutritionService.GetDiariesByUserIdAsync(_userSession.UserId);
                var progressTask = _nutritionService.GetDailyProgressByUserIdAsync(_userSession.UserId);
                await Task.WhenAll(diariesTask, progressTask).ConfigureAwait(false);

                var allDiaries = diariesTask.Result;
                var todayDiary = allDiaries.FirstOrDefault(d => d.Date == date);
                var todayProgress = progressTask.Result.FirstOrDefault(p => p.Date == date);

                // Load meals and liquids in parallel if diary exists
                List<MealDTO> meals = [];
                List<LiquidDTO> liquids = [];

                if (todayDiary != null)
                {
                    var mealsTask = _nutritionService.GetMealsByDiaryIdAsync(todayDiary.Id);
                    var liquidsTask = _nutritionService.GetLiquidsByDiaryIdAsync(todayDiary.Id);
                    await Task.WhenAll(mealsTask, liquidsTask).ConfigureAwait(false);
                    meals = mealsTask.Result;
                    liquids = liquidsTask.Result;
                }

                // Update UI on main thread in a single batch
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    TodayDiary = todayDiary;
                    DailyProgress = todayProgress;

                    if (todayProgress?.Goal != null)
                    {
                        CaloriesGoal = todayProgress.Goal.Calories > 0 ? todayProgress.Goal.Calories : 2000;
                        LiquidsGoalMl = todayProgress.Goal.QuantityMl > 0 ? todayProgress.Goal.QuantityMl : 2500;
                    }
                    else
                    {
                        CaloriesGoal = 2000;
                        LiquidsGoalMl = 2500;
                    }

                    Meals.ReplaceAll(meals);
                    Liquids.ReplaceAll(liquids);
                    CaloriesConsumed = meals.Sum(m => m.TotalCalories);
                    MealsCount = meals.Count;
                    LiquidsConsumedMl = liquids.Sum(l => l.Quantity);
                    LiquidsCount = liquids.Count;

                    // Calculate macronutrients from all meal foods
                    // Protein, Lipids, Carbohydrates are in grams per 100g; Sodium is in mg per 100g
                    var allFoods = meals.SelectMany(m => m.MealFoods).ToList();
                    TotalProtein = allFoods.Sum(f => (f.Protein ?? 0) * f.Quantity / 100m);
                    TotalCarbs = allFoods.Sum(f => (f.Carbohydrates ?? 0) * f.Quantity / 100m);
                    TotalLipids = allFoods.Sum(f => (f.Lipids ?? 0) * f.Quantity / 100m);
                    TotalSodium = allFoods.Sum(f => (f.Sodium ?? 0) * f.Quantity / 100m);
                    OnPropertyChanged(nameof(TotalProteinDisplay));
                    OnPropertyChanged(nameof(TotalCarbsDisplay));
                    OnPropertyChanged(nameof(TotalLipidsDisplay));
                    OnPropertyChanged(nameof(TotalSodiumDisplay));

                    IsLoadingData = false;
                    IsBusy = false;
                });

                _lastDataRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading nutrition data: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    IsLoadingData = false;
                    IsBusy = false;
                });
            }
        }

        public void InvalidateDataCache()
        {
            _lastDataRefresh = null;
            _loadingTask = null;
            _nutritionService.InvalidateAllCache();
        }


        private async Task EnsureDiaryExistsAsync()
        {
            if (TodayDiary != null) return;

            // Force reload from API (skip cache)
            InvalidateDataCache();
            await LoadDataAsync(forceRefresh: true);
            if (TodayDiary != null) return;

            // Diary doesn't exist, create it
            var dto = new CreateDiaryDTO(_userSession.UserId, _selectedDate);
            var (diaryId, error) = await _nutritionService.CreateDiaryAsync(dto);

            if (diaryId == null && error != null && error.Contains("Já existe"))
            {
                // Diary exists but cache was stale - just reload
                InvalidateDataCache();
                await LoadDataAsync(forceRefresh: true);
                return;
            }

            if (diaryId == null)
            {
                await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível criar o diário.", "OK");
                return;
            }

            InvalidateDataCache();
            await LoadDataAsync(forceRefresh: true);
        }

        private async Task OpenManageMealModalAsync()
        {
            await EnsureDiaryExistsAsync();
            if (TodayDiary == null) return;

            await Shell.Current.GoToAsync("ManageMealModal", new Dictionary<string, object>
            {
                { "DiaryId", TodayDiary.Id }
            });
        }

        private async Task OpenManageLiquidModalAsync()
        {
            await EnsureDiaryExistsAsync();
            if (TodayDiary == null) return;

            await Shell.Current.GoToAsync("ManageLiquidModal", new Dictionary<string, object>
            {
                { "DiaryId", TodayDiary.Id }
            });
        }

        private async Task OpenMealDetailAsync(MealDTO? meal)
        {
            if (meal == null) return;
            await Shell.Current.GoToAsync("MealDetailPage", new Dictionary<string, object>
            {
                { "Meal", meal }
            });
        }

        private async Task OpenDiaryDetailAsync()
        {
            if (TodayDiary == null) return;
            await Shell.Current.GoToAsync("DiaryDetailPage", new Dictionary<string, object>
            {
                { "Diary", TodayDiary }
            });
        }

        private async Task OpenDailyProgressAsync()
        {
            try
            {
                if (DailyProgress == null)
                    await LoadDataAsync(forceRefresh: true);

                if (DailyProgress == null)
                {
                    try
                    {
                        IsBusy = true;
                        var dto = new CreateDailyProgressDTO(_userSession.UserId, _selectedDate);
                        var progressId = await _nutritionService.CreateDailyProgressAsync(dto);
                        if (progressId == null)
                        {
                            await Shell.Current.DisplayAlert("Erro", "Não foi possível criar o progresso diário.", "OK");
                            return;
                        }
                        InvalidateDataCache();
                        await LoadDataAsync(forceRefresh: true);
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }

                if (DailyProgress == null)
                {
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível carregar o progresso diário.", "OK");
                    return;
                }

                await Shell.Current.GoToAsync("DailyProgressPage", new Dictionary<string, object>
                {
                    { "DailyProgress", DailyProgress }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening daily progress: {ex}");
                IsBusy = false;
                await Shell.Current.DisplayAlert("Erro", $"Erro ao abrir progresso diário: {ex.Message}", "OK");
            }
        }

        private async Task EditLiquidAsync(LiquidDTO? liquid)
        {
            if (liquid == null || TodayDiary == null) return;

            await Shell.Current.GoToAsync("ManageLiquidModal", new Dictionary<string, object>
            {
                { "DiaryId", TodayDiary.Id },
                { "Liquid", liquid }
            });
        }

        private async Task DeleteLiquidAsync(LiquidDTO? liquid)
        {
            if (liquid == null) return;

            var confirm = await Shell.Current.DisplayAlert(
                "Excluir Líquido",
                $"Deseja excluir {liquid.Name} ({liquid.Quantity} ml)?",
                "Excluir", "Cancelar");

            if (!confirm) return;

            var (success, error) = await _nutritionService.DeleteLiquidAsync(liquid.Id);
            if (success)
            {
                InvalidateDataCache();
                await LoadDataAsync(forceRefresh: true);
            }
            else
            {
                await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível excluir o líquido.", "OK");
            }
        }
    }
}
