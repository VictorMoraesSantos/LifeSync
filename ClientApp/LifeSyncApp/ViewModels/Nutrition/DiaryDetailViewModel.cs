using LifeSyncApp.DTOs.Nutrition.Diary;
using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.Services.Nutrition;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class DiaryDetailViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;
        private static readonly CultureInfo PtBr = new("pt-BR");

        private DiaryDTO? _diary;

        public DiaryDTO? Diary
        {
            get => _diary;
            set
            {
                SetProperty(ref _diary, value);
                if (value != null)
                {
                    SyncCollections(value);
                    OnPropertyChanged(nameof(DateTitle));
                    OnPropertyChanged(nameof(CaloriesTotal));
                    OnPropertyChanged(nameof(LiquidsTotal));
                }
            }
        }

        public string DateTitle => _diary != null
            ? $"Diário — {_diary.Date.ToString("dd MMM", PtBr)}"
            : "Diário";

        public string CaloriesTotal => _diary != null
            ? $"{_diary.TotalCalories:N0} kcal"
            : "0 kcal";

        public string LiquidsTotal => _diary != null
            ? $"{_diary.Liquids.Sum(l => l.Quantity):N0} ml"
            : "0 ml";

        public ObservableCollection<MealDTO> Meals { get; } = new();
        public ObservableCollection<LiquidDTO> Liquids { get; } = new();

        public ICommand GoBackCommand { get; }
        public ICommand OpenAddMealCommand { get; }
        public ICommand OpenEditMealCommand { get; }
        public ICommand DeleteMealCommand { get; }
        public ICommand OpenAddLiquidCommand { get; }
        public ICommand EditLiquidCommand { get; }
        public ICommand DeleteLiquidCommand { get; }
        public ICommand EditDateCommand { get; }
        public ICommand DeleteDiaryCommand { get; }

        public event EventHandler? DiaryDeleted;

        public DiaryDetailViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            OpenAddMealCommand = new Command(async () => await OpenAddMealAsync());
            OpenEditMealCommand = new Command<MealDTO>(async (m) => await OpenEditMealAsync(m));
            DeleteMealCommand = new Command<MealDTO>(async (m) => await DeleteMealAsync(m));
            OpenAddLiquidCommand = new Command(async () => await OpenAddLiquidAsync());
            EditLiquidCommand = new Command<LiquidDTO>(async (l) => await EditLiquidAsync(l));
            DeleteLiquidCommand = new Command<LiquidDTO>(async (l) => await DeleteLiquidAsync(l));
            EditDateCommand = new Command(async () => await EditDateAsync());
            DeleteDiaryCommand = new Command(async () => await DeleteDiaryAsync());
        }

        public async Task RefreshDiaryAsync()
        {
            if (_diary == null) return;
            try
            {
                IsBusy = true;
                var updated = await _nutritionService.GetDiaryByIdAsync(_diary.Id);
                if (updated != null)
                    Diary = updated;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing diary: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void SyncCollections(DiaryDTO diary)
        {
            Meals.Clear();
            foreach (var meal in diary.Meals)
                Meals.Add(meal);

            Liquids.Clear();
            foreach (var liquid in diary.Liquids)
                Liquids.Add(liquid);
        }

        private async Task OpenAddMealAsync()
        {
            if (_diary == null) return;
            try
            {
                await Shell.Current.GoToAsync("ManageMealModal", new Dictionary<string, object>
                {
                    { "DiaryId", _diary.Id }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async Task OpenEditMealAsync(MealDTO? meal)
        {
            if (meal == null || _diary == null) return;
            try
            {
                await Shell.Current.GoToAsync("ManageMealModal", new Dictionary<string, object>
                {
                    { "DiaryId", _diary.Id },
                    { "Meal", meal }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async Task DeleteMealAsync(MealDTO? meal)
        {
            if (meal == null) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar", $"Deseja remover a refeição '{meal.Name}'?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.DeleteMealAsync(meal.Id);
                if (success)
                    await RefreshDiaryAsync();
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", error ?? "Não foi possível remover a refeição.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task OpenAddLiquidAsync()
        {
            if (_diary == null) return;
            try
            {
                await Shell.Current.GoToAsync("ManageLiquidModal", new Dictionary<string, object>
                {
                    { "DiaryId", _diary.Id }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async Task EditLiquidAsync(LiquidDTO? liquid)
        {
            if (liquid == null || _diary == null) return;
            try
            {
                await Shell.Current.GoToAsync("ManageLiquidModal", new Dictionary<string, object>
                {
                    { "DiaryId", _diary.Id },
                    { "Liquid", liquid }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async Task DeleteLiquidAsync(LiquidDTO? liquid)
        {
            if (liquid == null) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar", $"Deseja remover '{liquid.Name}'?", "Sim", "Não");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.DeleteLiquidAsync(liquid.Id);
                if (success)
                    await RefreshDiaryAsync();
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", error ?? "Não foi possível remover o líquido.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task EditDateAsync()
        {
            if (_diary == null) return;

            var currentDate = _diary.Date.ToDateTime(TimeOnly.MinValue);
            var result = await Application.Current!.MainPage!.DisplayPromptAsync(
                "Editar data", "Nova data (dd/MM/yyyy):", initialValue: _diary.Date.ToString("dd/MM/yyyy"));

            if (string.IsNullOrWhiteSpace(result)) return;

            if (!DateOnly.TryParseExact(result, "dd/MM/yyyy", PtBr, DateTimeStyles.None, out var newDate))
            {
                await Application.Current.MainPage.DisplayAlert("Erro", "Formato de data inválido. Use dd/MM/yyyy.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.UpdateDiaryAsync(_diary.Id, new UpdateDiaryDTO(_diary.Id, newDate));
                if (success)
                    await RefreshDiaryAsync();
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", error ?? "Não foi possível atualizar a data.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteDiaryAsync()
        {
            if (_diary == null) return;

            var confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Confirmar", "Deseja excluir este diário? Esta ação não pode ser desfeita.", "Excluir", "Cancelar");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.DeleteDiaryAsync(_diary.Id);
                if (success)
                {
                    DiaryDeleted?.Invoke(this, EventArgs.Empty);
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    await Application.Current!.MainPage!.DisplayAlert("Erro", error ?? "Não foi possível excluir o diário.", "OK");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
