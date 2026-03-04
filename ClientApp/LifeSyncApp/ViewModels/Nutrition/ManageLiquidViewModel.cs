using LifeSyncApp.DTOs.Nutrition.Liquid;
using LifeSyncApp.Services.Nutrition;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Nutrition
{
    public class ManageLiquidViewModel : BaseViewModel
    {
        private readonly NutritionService _nutritionService;

        private bool _isEditing;
        private int _liquidId;
        private int _waterTypeId = 1;
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

        public string QuantityText
        {
            get => _quantityText;
            set => SetProperty(ref _quantityText, value);
        }

        public int DiaryId { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SetQuickQuantityCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageLiquidViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Adicionar Líquido";

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
            SetQuickQuantityCommand = new Command<string>(ml => QuantityText = ml);
        }

        public async Task InitializeAsync(int diaryId, LiquidDTO? liquid = null)
        {
            DiaryId = diaryId;

            // Resolve water type ID from API
            var types = await _nutritionService.GetLiquidTypesAsync();
            var waterType = types.FirstOrDefault(lt =>
                lt.Name.Equals("Água", StringComparison.OrdinalIgnoreCase));
            if (waterType != null)
                _waterTypeId = waterType.Id;

            if (liquid != null)
            {
                _liquidId = liquid.Id;
                IsEditing = true;
                QuantityText = liquid.Quantity.ToString();
            }
            else
            {
                _liquidId = 0;
                IsEditing = false;
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
                    var dto = new UpdateLiquidDTO(_liquidId, _waterTypeId, qty);
                    success = await _nutritionService.UpdateLiquidAsync(_liquidId, dto);
                }
                else
                {
                    var dto = new CreateLiquidDTO(DiaryId, _waterTypeId, qty);
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
