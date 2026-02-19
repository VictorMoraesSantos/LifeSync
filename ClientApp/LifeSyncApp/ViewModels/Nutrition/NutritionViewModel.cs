using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.Services.Nutrition;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class NutritionViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        private Task? _loadingTask;
        private DateTime? _lastDataRefresh;
        private const int CacheExpirationMinutes = 5;

        private DiaryDTO? _todayDiary;
        private DailyProgressDTO? _dailyProgress;
        private int _caloriesConsumed;
        private int _caloriesGoal = 2000;
        private int _liquidsConsumedMl;
        private int _liquidsGoalMl = 2000;
        private string _todayDate = string.Empty;

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

        public int CaloriesConsumed
        {
            get => _caloriesConsumed;
            set
            {
                SetProperty(ref _caloriesConsumed, value);
                OnPropertyChanged(nameof(CaloriesPercentage));
                OnPropertyChanged(nameof(CaloriesProgressValue));
                OnPropertyChanged(nameof(CaloriesDetail));
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
                OnPropertyChanged(nameof(CaloriesDetail));
            }
        }

        public int CaloriesPercentage => CaloriesGoal > 0
            ? Math.Min((int)Math.Round(CaloriesConsumed * 100.0 / CaloriesGoal), 100)
            : 0;

        public double CaloriesProgressValue => CaloriesGoal > 0
            ? Math.Min(CaloriesConsumed / (double)CaloriesGoal, 1.0)
            : 0.0;

        public string CaloriesDetail => $"{CaloriesConsumed} / {CaloriesGoal} kcal";

        public int LiquidsConsumedMl
        {
            get => _liquidsConsumedMl;
            set
            {
                SetProperty(ref _liquidsConsumedMl, value);
                OnPropertyChanged(nameof(LiquidsPercentage));
                OnPropertyChanged(nameof(LiquidsProgressValue));
                OnPropertyChanged(nameof(LiquidsDetail));
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
                OnPropertyChanged(nameof(LiquidsDetail));
            }
        }

        public int LiquidsPercentage => LiquidsGoalMl > 0
            ? Math.Min((int)Math.Round(LiquidsConsumedMl * 100.0 / LiquidsGoalMl), 100)
            : 0;

        public double LiquidsProgressValue => LiquidsGoalMl > 0
            ? Math.Min(LiquidsConsumedMl / (double)LiquidsGoalMl, 1.0)
            : 0.0;

        public string LiquidsDetail => $"{LiquidsConsumedMl}ml / {LiquidsGoalMl}ml";

        public string TodayDate
        {
            get => _todayDate;
            set => SetProperty(ref _todayDate, value);
        }

        public ObservableCollection<MealDTO> Meals { get; } = new();
        public ObservableCollection<LiquidDTO> Liquids { get; } = new();

        public ICommand LoadDataCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenManageMealModalCommand { get; }
        public ICommand OpenMealDetailCommand { get; }
        public ICommand DeleteMealCommand { get; }
        public ICommand OpenManageLiquidModalCommand { get; }
        public ICommand OpenEditLiquidModalCommand { get; }
        public ICommand DeleteLiquidCommand { get; }
        public ICommand AddQuickLiquidCommand { get; }
        public ICommand OpenManageGoalModalCommand { get; }

        public NutritionViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Nutrição";
            TodayDate = DateTime.Today.ToString("dd 'de' MMMM", new System.Globalization.CultureInfo("pt-BR"));

            LoadDataCommand = new Command(async () => await LoadDataAsync());
            RefreshCommand = new Command(async () => await LoadDataAsync(forceRefresh: true));
            OpenManageMealModalCommand = new Command(async () => await OpenManageMealModalAsync());
            OpenMealDetailCommand = new Command<MealDTO>(async (m) => await OpenMealDetailAsync(m));
            DeleteMealCommand = new Command<MealDTO>(async (m) => await DeleteMealAsync(m));
            OpenManageLiquidModalCommand = new Command(async () => await OpenManageLiquidModalAsync(null));
            OpenEditLiquidModalCommand = new Command<LiquidDTO>(async (l) => await OpenManageLiquidModalAsync(l));
            DeleteLiquidCommand = new Command<LiquidDTO>(async (l) => await DeleteLiquidAsync(l));
            AddQuickLiquidCommand = new Command<string>(async (ml) => await AddQuickLiquidAsync(ml));
            OpenManageGoalModalCommand = new Command(async () => await OpenManageGoalModalAsync());
        }

        public async Task InitializeAsync()
        {
            await LoadDataAsync();
        }

        public Task LoadDataAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsDataCacheExpired() && (Meals.Any() || Liquids.Any()))
                return Task.CompletedTask;

            // If a load is already in progress, await it instead of starting a new one
            if (_loadingTask != null && !_loadingTask.IsCompleted)
                return _loadingTask;

            _loadingTask = LoadDataInternalAsync();
            return _loadingTask;
        }

        private async Task LoadDataInternalAsync()
        {
            try
            {
                IsBusy = true;

                var today = DateOnly.FromDateTime(DateTime.Today);

                // Load diary, progress in parallel
                var diariesTask = _nutritionService.GetDiariesByUserIdAsync(_userId);
                var progressTask = _nutritionService.GetDailyProgressByUserIdAsync(_userId);
                await Task.WhenAll(diariesTask, progressTask);

                // Handle diary
                var todayDiary = diariesTask.Result.FirstOrDefault(d => d.Date == today);
                if (todayDiary == null)
                {
                    var diaryId = await _nutritionService.CreateDiaryAsync(new CreateDiaryDTO(_userId, today));
                    if (diaryId.HasValue)
                        todayDiary = await _nutritionService.GetDiaryByIdAsync(diaryId.Value);
                }
                TodayDiary = todayDiary;

                // Handle daily progress (goals)
                var todayProgress = progressTask.Result.FirstOrDefault(p => p.Date == today);
                if (todayProgress == null)
                {
                    var progressId = await _nutritionService.CreateDailyProgressAsync(
                        new CreateDailyProgressDTO(_userId, today));
                    if (progressId.HasValue)
                    {
                        // Reload the list to get the new entry
                        var updated = await _nutritionService.GetDailyProgressByUserIdAsync(_userId);
                        todayProgress = updated.FirstOrDefault(p => p.Date == today);
                    }
                }
                DailyProgress = todayProgress;

                // Apply goals from backend (or keep defaults)
                if (todayProgress?.Goal != null)
                {
                    CaloriesGoal = todayProgress.Goal.Calories > 0 ? todayProgress.Goal.Calories : 2000;
                    LiquidsGoalMl = todayProgress.Goal.QuantityMl > 0 ? todayProgress.Goal.QuantityMl : 2000;
                }

                // Load meals and liquids
                if (TodayDiary != null)
                {
                    var mealsTask = _nutritionService.GetMealsByDiaryIdAsync(TodayDiary.Id);
                    var liquidsTask = _nutritionService.GetLiquidsByDiaryIdAsync(TodayDiary.Id);
                    await Task.WhenAll(mealsTask, liquidsTask);

                    var meals = mealsTask.Result;
                    Meals.Clear();
                    foreach (var meal in meals)
                        Meals.Add(meal);
                    CaloriesConsumed = meals.Sum(m => m.TotalCalories);

                    var liquids = liquidsTask.Result;
                    Liquids.Clear();
                    foreach (var liquid in liquids)
                        Liquids.Add(liquid);
                    LiquidsConsumedMl = liquids.Sum(l => l.QuantityMl);
                }

                _lastDataRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading nutrition data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool IsDataCacheExpired()
        {
            if (_lastDataRefresh == null) return true;
            return (DateTime.Now - _lastDataRefresh.Value).TotalMinutes >= CacheExpirationMinutes;
        }

        public void InvalidateDataCache()
        {
            _lastDataRefresh = null;
            _loadingTask = null;
        }

        private async Task OpenManageMealModalAsync()
        {
            if (TodayDiary == null)
                await LoadDataAsync(forceRefresh: true);

            if (TodayDiary == null)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível carregar o diário. Verifique sua conexão.", "OK");
                return;
            }

            try
            {
                await Shell.Current.GoToAsync("ManageMealModal", new Dictionary<string, object>
                {
                    { "DiaryId", TodayDiary.Id }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Não foi possível abrir o modal: {ex.Message}", "OK");
            }
        }

        private async Task OpenMealDetailAsync(MealDTO? meal)
        {
            if (meal == null) return;
            try
            {
                await Shell.Current.GoToAsync("MealDetailPage", new Dictionary<string, object>
                {
                    { "Meal", meal }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Não foi possível abrir a refeição: {ex.Message}", "OK");
            }
        }

        private async Task DeleteMealAsync(MealDTO? meal)
        {
            if (meal == null) return;
            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar", $"Deseja remover a refeição '{meal.Name}'?", "Sim", "Não");
            if (!confirm) return;

            var success = await _nutritionService.DeleteMealAsync(meal.Id);
            if (success)
                await LoadDataAsync(forceRefresh: true);
        }

        private async Task OpenManageLiquidModalAsync(LiquidDTO? liquid)
        {
            if (TodayDiary == null)
                await LoadDataAsync(forceRefresh: true);

            if (TodayDiary == null)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível carregar o diário. Verifique sua conexão.", "OK");
                return;
            }

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "DiaryId", TodayDiary.Id }
                };
                if (liquid != null)
                    parameters["Liquid"] = liquid;

                await Shell.Current.GoToAsync("ManageLiquidModal", parameters);
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Não foi possível abrir o modal: {ex.Message}", "OK");
            }
        }

        private async Task DeleteLiquidAsync(LiquidDTO? liquid)
        {
            if (liquid == null) return;
            var success = await _nutritionService.DeleteLiquidAsync(liquid.Id);
            if (success)
                await LoadDataAsync(forceRefresh: true);
        }

        private async Task AddQuickLiquidAsync(string? mlStr)
        {
            if (!int.TryParse(mlStr, out var ml)) return;

            if (TodayDiary == null)
                await LoadDataAsync(forceRefresh: true);

            if (TodayDiary == null) return;

            var dto = new CreateLiquidDTO(TodayDiary.Id, "Água", ml, 0);
            var success = await _nutritionService.CreateLiquidAsync(dto);
            if (success)
                await LoadDataAsync(forceRefresh: true);
        }

        private async Task OpenManageGoalModalAsync()
        {
            if (DailyProgress == null)
                await LoadDataAsync(forceRefresh: true);

            if (DailyProgress == null)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível carregar o progresso diário. Verifique sua conexão.", "OK");
                return;
            }

            try
            {
                await Shell.Current.GoToAsync("ManageGoalModal", new Dictionary<string, object>
                {
                    { "DailyProgress", DailyProgress }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", $"Não foi possível abrir o modal: {ex.Message}", "OK");
            }
        }
    }
}
