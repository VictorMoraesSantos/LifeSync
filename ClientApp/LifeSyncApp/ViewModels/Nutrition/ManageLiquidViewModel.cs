using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.LiquidType;
using LifeSyncApp.Services.Nutrition;
using System.Collections.ObjectModel;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public partial class ManageLiquidViewModel : BaseViewModel
    {
        private readonly INutritionService _nutritionService;

        private int _liquidId;
        private bool _settingFromButton;

        [ObservableProperty]
        private LiquidTypeDTO? _selectedLiquidType;

        [ObservableProperty]
        private bool _isLoadingTypes;

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetProperty(ref _isEditing, value);
                Title = value ? "Editar Líquido" : "Adicionar Líquido";
            }
        }

        private string _quantityText = string.Empty;
        public string QuantityText
        {
            get => _quantityText;
            set
            {
                if (SetProperty(ref _quantityText, value) && !_settingFromButton)
                    SelectedQuickQuantity = null;
            }
        }

        [ObservableProperty]
        private string? _selectedQuickQuantity;

        public int DiaryId { get; private set; }

        public ObservableCollection<LiquidTypeDTO> LiquidTypes { get; } = new();

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageLiquidViewModel(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Adicionar Líquido";
        }

        [RelayCommand]
        private void SetQuickQuantity(string ml)
        {
            _settingFromButton = true;
            QuantityText = ml;
            SelectedQuickQuantity = ml;
            _settingFromButton = false;
        }

        [RelayCommand]
        private void SelectLiquidType(LiquidTypeDTO type)
        {
            SelectedLiquidType = type;
        }

        public async Task InitializeAsync(int diaryId, LiquidDTO? liquid = null)
        {
            DiaryId = diaryId;

            await LoadLiquidTypesAsync();

            if (liquid != null)
            {
                _liquidId = liquid.Id;
                IsEditing = true;
                _settingFromButton = true;
                QuantityText = liquid.Quantity.ToString();
                SelectedQuickQuantity = liquid.Quantity.ToString();
                _settingFromButton = false;

                var matchingType = LiquidTypes.FirstOrDefault(lt =>
                    lt.Name.Equals(liquid.Name, StringComparison.OrdinalIgnoreCase));
                SelectedLiquidType = matchingType ?? LiquidTypes.FirstOrDefault();
            }
            else
            {
                _liquidId = 0;
                IsEditing = false;
                _settingFromButton = true;
                QuantityText = "250";
                SelectedQuickQuantity = "250";
                _settingFromButton = false;

                var waterType = LiquidTypes.FirstOrDefault(lt =>
                    lt.Name.Equals("Água", StringComparison.OrdinalIgnoreCase));
                SelectedLiquidType = waterType ?? LiquidTypes.FirstOrDefault();
            }
        }

        private async Task LoadLiquidTypesAsync()
        {
            IsLoadingTypes = true;
            try
            {
                var types = await _nutritionService.GetLiquidTypesAsync();
                LiquidTypes.Clear();
                foreach (var type in types.OrderBy(t => t.Id))
                    LiquidTypes.Add(type);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading liquid types: {ex.Message}");
            }
            finally
            {
                IsLoadingTypes = false;
            }
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            if (IsBusy) return;

            if (SelectedLiquidType == null)
            {
                await Shell.Current.DisplayAlert("Atenção", "Selecione um tipo de líquido.", "OK");
                return;
            }

            if (!int.TryParse(_quantityText, out var qty) || qty <= 0)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe uma quantidade válida em ml.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                bool success;
                string? error;

                if (_isEditing)
                {
                    var dto = new UpdateLiquidDTO(_liquidId, SelectedLiquidType.Id, qty);
                    (success, error) = await _nutritionService.UpdateLiquidAsync(_liquidId, dto);
                }
                else
                {
                    var dto = new CreateLiquidDTO(DiaryId, SelectedLiquidType.Id, qty);
                    (success, error) = await _nutritionService.CreateLiquidAsync(dto);
                }

                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível salvar o líquido.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving liquid: {ex.Message}");
                await Shell.Current.DisplayAlert("Erro", $"Ocorreu um erro ao salvar: {ex.Message}", "OK");
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
