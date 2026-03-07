using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.DTOs.Nutrition.LiquidType;
using LifeSyncApp.Services.Nutrition;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class ManageLiquidViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;

        private bool _isEditing;
        private int _liquidId;
        private string _quantityText = string.Empty;
        private string? _selectedQuickQuantity;
        private bool _settingFromButton;
        private LiquidTypeDTO? _selectedLiquidType;
        private bool _isLoadingTypes;

        public ObservableCollection<LiquidTypeDTO> LiquidTypes { get; } = new();

        public LiquidTypeDTO? SelectedLiquidType
        {
            get => _selectedLiquidType;
            set => SetProperty(ref _selectedLiquidType, value);
        }

        public bool IsLoadingTypes
        {
            get => _isLoadingTypes;
            set => SetProperty(ref _isLoadingTypes, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetProperty(ref _isEditing, value);
                Title = value ? "Editar Líquido" : "Adicionar Líquido";
            }
        }

        public string QuantityText
        {
            get => _quantityText;
            set
            {
                if (SetProperty(ref _quantityText, value) && !_settingFromButton)
                    SelectedQuickQuantity = null;
            }
        }

        public string? SelectedQuickQuantity
        {
            get => _selectedQuickQuantity;
            set => SetProperty(ref _selectedQuickQuantity, value);
        }

        public int DiaryId { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SetQuickQuantityCommand { get; }
        public ICommand SelectLiquidTypeCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageLiquidViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Adicionar Líquido";

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
            SetQuickQuantityCommand = new Command<string>(ml =>
            {
                _settingFromButton = true;
                QuantityText = ml;
                SelectedQuickQuantity = ml;
                _settingFromButton = false;
            });
            SelectLiquidTypeCommand = new Command<LiquidTypeDTO>(type =>
            {
                SelectedLiquidType = type;
            });
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

                // Select the liquid type that matches the liquid's name
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

                // Default to "Água" if available
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
    }
}
