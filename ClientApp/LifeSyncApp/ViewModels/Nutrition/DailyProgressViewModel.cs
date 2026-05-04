using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;
using System.Globalization;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class DailyProgressViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;
        private readonly IUserSession _userSession;
        private static readonly CultureInfo PtBr = new("pt-BR");

        [ObservableProperty]
        private bool _isLoadingData = true;

        [ObservableProperty]
        private DailyProgressDTO? _currentProgress;

        [ObservableProperty]
        private string _dateLabel = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CaloriesPercentage))]
        [NotifyPropertyChangedFor(nameof(CaloriesProgressValue))]
        [NotifyPropertyChangedFor(nameof(CaloriesInfo))]
        private int _caloriesConsumed;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CaloriesPercentage))]
        [NotifyPropertyChangedFor(nameof(CaloriesProgressValue))]
        [NotifyPropertyChangedFor(nameof(CaloriesInfo))]
        private int _caloriesGoal = 2000;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LiquidsPercentage))]
        [NotifyPropertyChangedFor(nameof(LiquidsProgressValue))]
        [NotifyPropertyChangedFor(nameof(LiquidsInfo))]
        private int _liquidsConsumedMl;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LiquidsPercentage))]
        [NotifyPropertyChangedFor(nameof(LiquidsProgressValue))]
        [NotifyPropertyChangedFor(nameof(LiquidsInfo))]
        private int _liquidsGoalMl = 2500;

        [ObservableProperty]
        private string _caloriesGoalText = "2000";

        [ObservableProperty]
        private string _liquidsGoalText = "2500";

        private DateOnly _selectedDate;
        public DateOnly SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    UpdateDateLabel();
                    _ = LoadProgressAsync();
                }
            }
        }

        private DateOnly _initialDate;
        private int _initialCaloriesConsumed;
        private int _initialLiquidsConsumedMl;

        public int CaloriesPercentage => CaloriesGoal > 0
            ? Math.Min((int)Math.Round(CaloriesConsumed * 100.0 / CaloriesGoal), 100) : 0;
        public double CaloriesProgressValue => CaloriesGoal > 0
            ? Math.Min(CaloriesConsumed / (double)CaloriesGoal, 1.0) : 0.0;
        public string CaloriesInfo => $"{CaloriesConsumed} / {CaloriesGoal}";

        public int LiquidsPercentage => LiquidsGoalMl > 0
            ? Math.Min((int)Math.Round(LiquidsConsumedMl * 100.0 / LiquidsGoalMl), 100) : 0;
        public double LiquidsProgressValue => LiquidsGoalMl > 0
            ? Math.Min(LiquidsConsumedMl / (double)LiquidsGoalMl, 1.0) : 0.0;
        public string LiquidsInfo => $"{LiquidsConsumedMl} / {LiquidsGoalMl}";

        public ObservableCollection<DailyProgressDTO> RecentHistory { get; } = new();

        public DailyProgressViewModel(INutritionService nutritionService, IUserSession userSession)
        {
            _nutritionService = nutritionService;
            _userSession = userSession;
            Title = "Progresso Diário";
        }

        public void Initialize(DailyProgressDTO? progress, int caloriesConsumed = 0, int liquidsConsumedMl = 0)
        {
            _selectedDate = progress?.Date ?? DateOnly.FromDateTime(DateTime.Today);
            _initialDate = _selectedDate;
            _initialCaloriesConsumed = caloriesConsumed;
            _initialLiquidsConsumedMl = liquidsConsumedMl;
            UpdateDateLabel();

            CurrentProgress = progress;
            CaloriesConsumed = caloriesConsumed;
            LiquidsConsumedMl = liquidsConsumedMl;

            if (progress?.Goal != null)
            {
                CaloriesGoal = progress.Goal.Calories;
                LiquidsGoalMl = progress.Goal.QuantityMl;
                CaloriesGoalText = progress.Goal.Calories.ToString();
                LiquidsGoalText = progress.Goal.QuantityMl.ToString();
            }
            else
            {
                CaloriesGoal = 2000;
                LiquidsGoalMl = 2500;
                CaloriesGoalText = "2000";
                LiquidsGoalText = "2500";
            }

            IsLoadingData = false;
            _ = LoadHistoryAsync();
        }

        private void UpdateDateLabel()
        {
            DateLabel = _selectedDate.ToString("dd 'de' MMMM, yyyy", PtBr);
        }

        [RelayCommand]
        private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private void PreviousDay() => SelectedDate = SelectedDate.AddDays(-1);

        [RelayCommand]
        private void NextDay() => SelectedDate = SelectedDate.AddDays(1);

        private async Task LoadProgressAsync()
        {
            try
            {
                IsBusy = true;
                var progresses = await _nutritionService.GetDailyProgressByUserIdAsync(_userSession.UserId);
                var progress = progresses.FirstOrDefault(p => p.Date == _selectedDate);
                CurrentProgress = progress;

                if (_selectedDate == _initialDate)
                {
                    CaloriesConsumed = _initialCaloriesConsumed;
                    LiquidsConsumedMl = _initialLiquidsConsumedMl;
                }
                else
                {
                    CaloriesConsumed = progress?.CaloriesConsumed ?? 0;
                    LiquidsConsumedMl = progress?.LiquidsConsumedMl ?? 0;
                }

                if (progress?.Goal != null)
                {
                    CaloriesGoal = progress.Goal.Calories;
                    LiquidsGoalMl = progress.Goal.QuantityMl;
                    CaloriesGoalText = progress.Goal.Calories.ToString();
                    LiquidsGoalText = progress.Goal.QuantityMl.ToString();
                }
                else
                {
                    var latestGoal = progresses
                        .Where(p => p.Goal != null && p.Date <= _selectedDate)
                        .OrderByDescending(p => p.Date)
                        .FirstOrDefault()?.Goal;

                    if (latestGoal != null)
                    {
                        CaloriesGoal = latestGoal.Calories;
                        LiquidsGoalMl = latestGoal.QuantityMl;
                        CaloriesGoalText = latestGoal.Calories.ToString();
                        LiquidsGoalText = latestGoal.QuantityMl.ToString();
                    }
                    else
                    {
                        CaloriesGoal = 2000;
                        LiquidsGoalMl = 2500;
                        CaloriesGoalText = "2000";
                        LiquidsGoalText = "2500";
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadHistoryAsync()
        {
            try
            {
                var progresses = await _nutritionService.GetDailyProgressByUserIdAsync(_userSession.UserId);
                var recent = progresses
                    .OrderByDescending(p => p.Date)
                    .Take(7)
                    .ToList();

                RecentHistory.Clear();
                foreach (var p in recent)
                {
                    if (p.Date == _initialDate)
                    {
                        RecentHistory.Add(p with
                        {
                            CaloriesConsumed = _initialCaloriesConsumed,
                            LiquidsConsumedMl = _initialLiquidsConsumedMl
                        });
                    }
                    else
                    {
                        RecentHistory.Add(p);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SaveGoalAsync()
        {
            if (CurrentProgress == null || IsBusy) return;

            if (!int.TryParse(CaloriesGoalText, out var calGoal) || calGoal <= 0)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe uma meta de calorias válida.", "OK");
                return;
            }
            if (!int.TryParse(LiquidsGoalText, out var liqGoal) || liqGoal <= 0)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe uma meta de líquidos válida.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var goal = new DailyGoalDTO(calGoal, liqGoal);
                var dto = new SetGoalDTO(goal);
                var (success, error) = await _nutritionService.SetGoalAsync(CurrentProgress.Id, dto);
                if (success)
                {
                    CaloriesGoal = calGoal;
                    LiquidsGoalMl = liqGoal;
                    await LoadHistoryAsync();
                    await Shell.Current.DisplayAlert("Sucesso", "Meta salva com sucesso!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível salvar a meta.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ResetGoalAsync()
        {
            CaloriesGoalText = "2000";
            LiquidsGoalText = "2500";
            await SaveGoalAsync();
        }
    }
}
