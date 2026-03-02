using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.Services.Nutrition;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class DailyProgressViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;
        private readonly IUserSession _userSession;
        private static readonly CultureInfo PtBr = new("pt-BR");

        private DailyProgressDTO? _dailyProgress;
        private DateOnly _selectedDate;
        private string _dateLabel = string.Empty;
        private int _caloriesConsumed;
        private int _caloriesGoal = 2000;
        private int _liquidsConsumedMl;
        private int _liquidsGoalMl = 2500;
        private string _caloriesGoalText = "2000";
        private string _liquidsGoalText = "2500";

        public DailyProgressDTO? CurrentProgress
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
                    _ = LoadProgressAsync();
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
                OnPropertyChanged(nameof(CaloriesInfo));
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
                OnPropertyChanged(nameof(CaloriesInfo));
            }
        }

        public int LiquidsConsumedMl
        {
            get => _liquidsConsumedMl;
            set
            {
                SetProperty(ref _liquidsConsumedMl, value);
                OnPropertyChanged(nameof(LiquidsPercentage));
                OnPropertyChanged(nameof(LiquidsProgressValue));
                OnPropertyChanged(nameof(LiquidsInfo));
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
                OnPropertyChanged(nameof(LiquidsInfo));
            }
        }

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

        public string CaloriesGoalText
        {
            get => _caloriesGoalText;
            set => SetProperty(ref _caloriesGoalText, value);
        }

        public string LiquidsGoalText
        {
            get => _liquidsGoalText;
            set => SetProperty(ref _liquidsGoalText, value);
        }

        public ObservableCollection<DailyProgressDTO> RecentHistory { get; } = new();

        public ICommand GoBackCommand { get; }
        public ICommand PreviousDayCommand { get; }
        public ICommand NextDayCommand { get; }
        public ICommand SaveGoalCommand { get; }
        public ICommand ResetGoalCommand { get; }

        public DailyProgressViewModel(NutritionService nutritionService, IUserSession userSession)
        {
            _nutritionService = nutritionService;
            _userSession = userSession;
            Title = "Progresso Diário";

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            PreviousDayCommand = new Command(() => SelectedDate = SelectedDate.AddDays(-1));
            NextDayCommand = new Command(() => SelectedDate = SelectedDate.AddDays(1));
            SaveGoalCommand = new Command(async () => await SaveGoalAsync());
            ResetGoalCommand = new Command(async () => await ResetGoalAsync());
        }

        public void Initialize(DailyProgressDTO? progress)
        {
            _selectedDate = progress?.Date ?? DateOnly.FromDateTime(DateTime.Today);
            UpdateDateLabel();

            if (progress != null)
            {
                CurrentProgress = progress;
                CaloriesConsumed = progress.CaloriesConsumed ?? 0;
                LiquidsConsumedMl = progress.LiquidsConsumedMl ?? 0;
                if (progress.Goal != null)
                {
                    CaloriesGoal = progress.Goal.Calories;
                    LiquidsGoalMl = progress.Goal.QuantityMl;
                    CaloriesGoalText = progress.Goal.Calories.ToString();
                    LiquidsGoalText = progress.Goal.QuantityMl.ToString();
                }
            }

            _ = LoadHistoryAsync();
        }

        private void UpdateDateLabel()
        {
            DateLabel = _selectedDate.ToString("dd 'de' MMMM, yyyy", PtBr);
        }

        private async Task LoadProgressAsync()
        {
            try
            {
                IsBusy = true;
                var progresses = await _nutritionService.GetDailyProgressByUserIdAsync(_userSession.UserId);
                var progress = progresses.FirstOrDefault(p => p.Date == _selectedDate);
                if (progress != null)
                {
                    CurrentProgress = progress;
                    CaloriesConsumed = progress.CaloriesConsumed ?? 0;
                    LiquidsConsumedMl = progress.LiquidsConsumedMl ?? 0;
                    if (progress.Goal != null)
                    {
                        CaloriesGoal = progress.Goal.Calories;
                        LiquidsGoalMl = progress.Goal.QuantityMl;
                        CaloriesGoalText = progress.Goal.Calories.ToString();
                        LiquidsGoalText = progress.Goal.QuantityMl.ToString();
                    }
                }
                else
                {
                    CurrentProgress = null;
                    CaloriesConsumed = 0;
                    LiquidsConsumedMl = 0;
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
                    RecentHistory.Add(p);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
            }
        }

        private async Task SaveGoalAsync()
        {
            if (_dailyProgress == null || IsBusy) return;

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
                var success = await _nutritionService.SetGoalAsync(_dailyProgress.Id, dto);
                if (success)
                {
                    CaloriesGoal = calGoal;
                    LiquidsGoalMl = liqGoal;
                    await Shell.Current.DisplayAlert("Sucesso", "Meta salva com sucesso!", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ResetGoalAsync()
        {
            CaloriesGoalText = "2000";
            LiquidsGoalText = "2500";
            await SaveGoalAsync();
        }
    }
}
