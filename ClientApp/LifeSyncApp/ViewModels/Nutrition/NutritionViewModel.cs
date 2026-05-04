using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.Helpers;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.UserSession;
using System.Globalization;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class NutritionViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;
        private readonly IUserSession _userSession;
        private static readonly CultureInfo PtBr = new("pt-BR");

        private Task? _loadingTask;
        private DateTime? _lastDataRefresh;

        [ObservableProperty]
        private bool _isLoadingData;

        [ObservableProperty]
        private DiaryDTO? _todayDiary;

        [ObservableProperty]
        private DailyProgressDTO? _dailyProgress;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CaloriesPercentage))]
        [NotifyPropertyChangedFor(nameof(CaloriesProgressValue))]
        [NotifyPropertyChangedFor(nameof(CaloriesDisplay))]
        private int _caloriesConsumed;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CaloriesPercentage))]
        [NotifyPropertyChangedFor(nameof(CaloriesProgressValue))]
        [NotifyPropertyChangedFor(nameof(CaloriesDisplay))]
        [NotifyPropertyChangedFor(nameof(CaloriesGoalDisplay))]
        private int _caloriesGoal = 2000;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LiquidsPercentage))]
        [NotifyPropertyChangedFor(nameof(LiquidsProgressValue))]
        [NotifyPropertyChangedFor(nameof(LiquidsDisplay))]
        private int _liquidsConsumedMl;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LiquidsPercentage))]
        [NotifyPropertyChangedFor(nameof(LiquidsProgressValue))]
        [NotifyPropertyChangedFor(nameof(LiquidsDisplay))]
        [NotifyPropertyChangedFor(nameof(LiquidsGoalDisplay))]
        private int _liquidsGoalMl = 2500;

        [ObservableProperty]
        private string _dateLabel = string.Empty;

        [ObservableProperty]
        private int _mealsCount;

        [ObservableProperty]
        private int _liquidsCount;

        private DateOnly _selectedDate;
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

        public int CaloriesPercentage => CaloriesGoal > 0
            ? Math.Min((int)Math.Round(CaloriesConsumed * 100.0 / CaloriesGoal), 100)
            : 0;

        public double CaloriesProgressValue => CaloriesGoal > 0
            ? Math.Min(CaloriesConsumed / (double)CaloriesGoal, 1.0)
            : 0.0;

        public string CaloriesDisplay => $"{CaloriesConsumed:N0}";
        public string CaloriesGoalDisplay => $"de {CaloriesGoal:N0} kcal";

        public int LiquidsPercentage => LiquidsGoalMl > 0
            ? Math.Min((int)Math.Round(LiquidsConsumedMl * 100.0 / LiquidsGoalMl), 100)
            : 0;

        public double LiquidsProgressValue => LiquidsGoalMl > 0
            ? Math.Min(LiquidsConsumedMl / (double)LiquidsGoalMl, 1.0)
            : 0.0;

        public string LiquidsDisplay => $"{LiquidsConsumedMl:N0}";
        public string LiquidsGoalDisplay => $"de {LiquidsGoalMl:N0} ml";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalProteinDisplay))]
        private decimal _totalProtein;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalCarbsDisplay))]
        private decimal _totalCarbs;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalLipidsDisplay))]
        private decimal _totalLipids;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TotalSodiumDisplay))]
        private decimal _totalSodium;

        public string TotalProteinDisplay => $"{TotalProtein:0.##}g";
        public string TotalCarbsDisplay => $"{TotalCarbs:0.##}g";
        public string TotalLipidsDisplay => $"{TotalLipids:0.##}g";
        public string TotalSodiumDisplay => $"{TotalSodium / 1000m:0.##}g";

        public SafeObservableCollection<MealDTO> Meals { get; } = new();
        public SafeObservableCollection<LiquidDTO> Liquids { get; } = new();

        public NutritionViewModel(INutritionService nutritionService, IUserSession userSession)
        {
            _nutritionService = nutritionService;
            _userSession = userSession;
            Title = "Nutrição";
            _selectedDate = DateOnly.FromDateTime(DateTime.Today);
            UpdateDateLabel();
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

                var diariesTask = _nutritionService.GetDiariesByUserIdAsync(_userSession.UserId);
                var progressTask = _nutritionService.GetDailyProgressByUserIdAsync(_userSession.UserId);
                await Task.WhenAll(diariesTask, progressTask).ConfigureAwait(false);

                var allDiaries = diariesTask.Result;
                var todayDiary = allDiaries.FirstOrDefault(d => d.Date == date);
                var todayProgress = progressTask.Result.FirstOrDefault(p => p.Date == date);

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

                    var allFoods = meals.SelectMany(m => m.MealFoods).ToList();
                    TotalProtein = allFoods.Sum(f => (f.Protein ?? 0) * f.Quantity / 100m);
                    TotalCarbs = allFoods.Sum(f => (f.Carbohydrates ?? 0) * f.Quantity / 100m);
                    TotalLipids = allFoods.Sum(f => (f.Lipids ?? 0) * f.Quantity / 100m);
                    TotalSodium = allFoods.Sum(f => (f.Sodium ?? 0) * f.Quantity / 100m);

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

        [RelayCommand]
        private async Task RefreshAsync() => await LoadDataAsync(forceRefresh: true);

        [RelayCommand]
        private void PreviousDay() => SelectedDate = SelectedDate.AddDays(-1);

        [RelayCommand]
        private void NextDay() => SelectedDate = SelectedDate.AddDays(1);

        private async Task EnsureDiaryExistsAsync()
        {
            if (TodayDiary != null) return;

            var date = _selectedDate;

            var diary = await _nutritionService.GetDiaryByDateAsync(_userSession.UserId, date);
            if (diary != null)
            {
                TodayDiary = diary;
                return;
            }

            var dto = new CreateDiaryDTO(_userSession.UserId, date);
            var (diaryId, error) = await _nutritionService.CreateDiaryAsync(dto);

            if (diaryId == null)
            {
                if (error != null && error.Contains("Já existe"))
                {
                    diary = await _nutritionService.GetDiaryByDateAsync(_userSession.UserId, date);
                    if (diary != null)
                    {
                        TodayDiary = diary;
                        return;
                    }
                }
                await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível criar o diário.", "OK");
                return;
            }

            TodayDiary = new DiaryDTO(
                diaryId.Value,
                _userSession.UserId,
                date,
                DateTime.Now,
                null,
                0,
                new List<MealDTO>(),
                new List<LiquidDTO>()
            );
        }

        [RelayCommand]
        private async Task OpenManageMealModalAsync()
        {
            await EnsureDiaryExistsAsync();
            if (TodayDiary == null) return;

            await Shell.Current.GoToAsync(AppRoutes.ManageMealModal, new Dictionary<string, object>
            {
                { "DiaryId", TodayDiary.Id }
            });
        }

        [RelayCommand]
        private async Task OpenManageLiquidModalAsync()
        {
            await EnsureDiaryExistsAsync();
            if (TodayDiary == null) return;

            await Shell.Current.GoToAsync(AppRoutes.ManageLiquidModal, new Dictionary<string, object>
            {
                { "DiaryId", TodayDiary.Id }
            });
        }

        [RelayCommand]
        private async Task OpenMealDetailAsync(MealDTO? meal)
        {
            if (meal == null) return;
            await Shell.Current.GoToAsync(AppRoutes.MealDetailPage, new Dictionary<string, object>
            {
                { "Meal", meal }
            });
        }

        [RelayCommand]
        private async Task OpenDiaryDetailAsync()
        {
            if (TodayDiary == null) return;
            await Shell.Current.GoToAsync(AppRoutes.DiaryDetailPage, new Dictionary<string, object>
            {
                { "Diary", TodayDiary }
            });
        }

        [RelayCommand]
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

                await Shell.Current.GoToAsync(AppRoutes.DailyProgressPage, new Dictionary<string, object>
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

        [RelayCommand]
        private async Task OpenDiaryHistoryAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.DiaryHistoryPage);
        }

        [RelayCommand]
        private async Task EditLiquidAsync(LiquidDTO? liquid)
        {
            if (liquid == null || TodayDiary == null) return;

            await Shell.Current.GoToAsync(AppRoutes.ManageLiquidModal, new Dictionary<string, object>
            {
                { "DiaryId", TodayDiary.Id },
                { "Liquid", liquid }
            });
        }

        [RelayCommand]
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
