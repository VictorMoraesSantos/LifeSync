using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.Models.Nutrition.Diary;
using LifeSyncApp.Models.Nutrition.Liquid;
using LifeSyncApp.Models.Nutrition.Meal;
using LifeSyncApp.Services.Nutrition;
using System.Collections.ObjectModel;
using System.Globalization;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class DiaryDetailViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;
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

        public event EventHandler? DiaryDeleted;

        public DiaryDetailViewModel(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
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

        [RelayCommand]
        private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

        [RelayCommand]
        private async Task OpenAddMealAsync()
        {
            if (_diary == null) return;
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.ManageMealModal, new Dictionary<string, object>
                {
                    { "DiaryId", _diary.Id }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DiaryDetail] OpenAddMeal error: {ex}");
                if (Shell.Current?.CurrentPage is Page currentPage)
                    await currentPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task OpenEditMealAsync(MealDTO? m)
        {
            if (m == null || _diary == null) return;
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.ManageMealModal, new Dictionary<string, object>
                {
                    { "DiaryId", _diary.Id },
                    { "Meal", m }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DiaryDetail] OpenEditMeal error: {ex}");
                await ShowAlertAsync("Erro", ex.Message);
            }
        }

        [RelayCommand]
        private async Task DeleteMealAsync(MealDTO? m)
        {
            if (m == null) return;

            var confirm = await ShowConfirmAsync(
                "Confirmar", $"Deseja remover a refeição '{m.Name}'?");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.DeleteMealAsync(m.Id);
                if (success)
                    await RefreshDiaryAsync();
                else
                    await ShowAlertAsync("Erro", error ?? "Não foi possível remover a refeição.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task OpenAddLiquidAsync()
        {
            if (_diary == null) return;
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.ManageLiquidModal, new Dictionary<string, object>
                {
                    { "DiaryId", _diary.Id }
                });
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Erro", ex.Message);
            }
        }

        [RelayCommand]
        private async Task EditLiquidAsync(LiquidDTO? l)
        {
            if (l == null || _diary == null) return;
            try
            {
                await Shell.Current.GoToAsync(AppRoutes.ManageLiquidModal, new Dictionary<string, object>
                {
                    { "DiaryId", _diary.Id },
                    { "Liquid", l }
                });
            }
            catch (Exception ex)
            {
                await ShowAlertAsync("Erro", ex.Message);
            }
        }

        [RelayCommand]
        private async Task DeleteLiquidAsync(LiquidDTO? l)
        {
            if (l == null) return;

            var confirm = await ShowConfirmAsync(
                "Confirmar", $"Deseja remover '{l.Name}'?");
            if (!confirm) return;

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.DeleteLiquidAsync(l.Id);
                if (success)
                    await RefreshDiaryAsync();
                else
                    await ShowAlertAsync("Erro", error ?? "Não foi possível remover o líquido.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task EditDateAsync()
        {
            if (_diary == null) return;

            var result = await ShowPromptAsync(
                "Editar data", "Nova data (dd/MM/yyyy):", initialValue: _diary.Date.ToString("dd/MM/yyyy"));

            if (string.IsNullOrWhiteSpace(result)) return;

            if (!DateOnly.TryParseExact(result, "dd/MM/yyyy", PtBr, DateTimeStyles.None, out var newDate))
            {
                await ShowAlertAsync("Erro", "Formato de data inválido. Use dd/MM/yyyy.");
                return;
            }

            IsBusy = true;
            try
            {
                var (success, error) = await _nutritionService.UpdateDiaryAsync(_diary.Id, new UpdateDiaryDTO(_diary.Id, newDate));
                if (success)
                    await RefreshDiaryAsync();
                else
                    await ShowAlertAsync("Erro", error ?? "Não foi possível atualizar a data.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteDiaryAsync()
        {
            if (_diary == null) return;

            var confirm = await ShowConfirmAsync(
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
                    await ShowAlertAsync("Erro", error ?? "Não foi possível excluir o diário.");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
