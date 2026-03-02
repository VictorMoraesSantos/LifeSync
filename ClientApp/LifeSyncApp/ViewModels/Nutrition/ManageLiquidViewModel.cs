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
        private int _selectedLiquidTypeId;
        private string _quantityText = string.Empty;

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetProperty(ref _isEditing, value);
                Title = value ? "Editar Líquido" : "Adicionar Líquido";
            }
        }

        public ObservableCollection<LiquidTypeDTO> LiquidTypes { get; } = new();

        public int SelectedLiquidTypeId
        {
            get => _selectedLiquidTypeId;
            set => SetProperty(ref _selectedLiquidTypeId, value);
        }

        public string QuantityText
        {
            get => _quantityText;
            set => SetProperty(ref _quantityText, value);
        }

        public int DiaryId { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SelectLiquidTypeCommand { get; }
        public ICommand SetQuickQuantityCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageLiquidViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Adicionar Líquido";

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
            SelectLiquidTypeCommand = new Command<LiquidTypeDTO>(lt => SelectedLiquidTypeId = lt.Id);
            SetQuickQuantityCommand = new Command<string>(ml => QuantityText = ml);
        }

        public async Task InitializeAsync(int diaryId, LiquidDTO? liquid = null)
        {
            DiaryId = diaryId;

            // Load liquid types
            var types = await _nutritionService.GetLiquidTypesAsync();
            LiquidTypes.Clear();
            if (!types.Any())
            {
                // Fallback defaults
                LiquidTypes.Add(new LiquidTypeDTO(1, "Água"));
                LiquidTypes.Add(new LiquidTypeDTO(2, "Suco"));
                LiquidTypes.Add(new LiquidTypeDTO(3, "Café"));
                LiquidTypes.Add(new LiquidTypeDTO(4, "Chá"));
                LiquidTypes.Add(new LiquidTypeDTO(5, "Refrigerante"));
            }
            else
            {
                foreach (var lt in types)
                    LiquidTypes.Add(lt);
            }

            if (liquid != null)
            {
                _liquidId = liquid.Id;
                IsEditing = true;
                // Try to find the matching liquid type
                var matchingType = LiquidTypes.FirstOrDefault(lt =>
                    lt.Name.Equals(liquid.Name, StringComparison.OrdinalIgnoreCase));
                SelectedLiquidTypeId = matchingType?.Id ?? LiquidTypes.First().Id;
                QuantityText = liquid.Quantity.ToString();
            }
            else
            {
                _liquidId = 0;
                IsEditing = false;
                SelectedLiquidTypeId = LiquidTypes.First().Id;
                QuantityText = "250";
            }
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;

            if (!int.TryParse(_quantityText, out var qty) || qty <= 0)
            {
                await Shell.Current.DisplayAlert("Atenção", "Informe uma quantidade válida em ml.", "OK");
                return;
            }

            IsBusy = true;
            try
            {
                bool success;
                if (_isEditing)
                {
                    var dto = new UpdateLiquidDTO(_liquidId, SelectedLiquidTypeId, qty);
                    success = await _nutritionService.UpdateLiquidAsync(_liquidId, dto);
                }
                else
                {
                    var dto = new CreateLiquidDTO(DiaryId, SelectedLiquidTypeId, qty);
                    success = await _nutritionService.CreateLiquidAsync(dto);
                }

                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Shell.Current.DisplayAlert("Erro", "Não foi possível salvar o líquido.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving liquid: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
