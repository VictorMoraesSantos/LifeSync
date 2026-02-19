using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.Services.Nutrition;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class ManageGoalViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;

        private int _dailyProgressId;
        private string _caloriesGoalText = string.Empty;
        private string _liquidsGoalMlText = string.Empty;

        public string CaloriesGoalText
        {
            get => _caloriesGoalText;
            set => SetProperty(ref _caloriesGoalText, value);
        }

        public string LiquidsGoalMlText
        {
            get => _liquidsGoalMlText;
            set => SetProperty(ref _liquidsGoalMlText, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageGoalViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Definir Metas";

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize(DailyProgressDTO dailyProgress)
        {
            _dailyProgressId = dailyProgress.Id;
            CaloriesGoalText = (dailyProgress.Goal?.Calories > 0 ? dailyProgress.Goal.Calories : 2000).ToString();
            LiquidsGoalMlText = (dailyProgress.Goal?.QuantityMl > 0 ? dailyProgress.Goal.QuantityMl : 2000).ToString();
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;

            if (!int.TryParse(_caloriesGoalText, out var calories) || calories <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert("Atenção", "Informe uma meta de calorias válida.", "OK");
                return;
            }

            if (!int.TryParse(_liquidsGoalMlText, out var liquidsml) || liquidsml <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert("Atenção", "Informe uma meta de hidratação válida.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var dto = new SetGoalDTO(new DailyGoalDTO(calories, liquidsml));
                var success = await _nutritionService.SetGoalAsync(_dailyProgressId, dto);
                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível salvar as metas.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving goal: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
