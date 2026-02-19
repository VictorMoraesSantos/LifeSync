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
        private string _name = string.Empty;
        private string _quantityMlText = string.Empty;
        private string _caloriesPerMlText = "0";

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                SetProperty(ref _isEditing, value);
                Title = value ? "Editar Líquido" : "Adicionar Líquido";
            }
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string QuantityMlText
        {
            get => _quantityMlText;
            set
            {
                SetProperty(ref _quantityMlText, value);
                OnPropertyChanged(nameof(TotalCalories));
            }
        }

        public string CaloriesPerMlText
        {
            get => _caloriesPerMlText;
            set
            {
                SetProperty(ref _caloriesPerMlText, value);
                OnPropertyChanged(nameof(TotalCalories));
            }
        }

        public int TotalCalories
        {
            get
            {
                if (int.TryParse(_quantityMlText, out var qty) && int.TryParse(_caloriesPerMlText, out var cal))
                    return qty * cal;
                return 0;
            }
        }

        public int DiaryId { get; private set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageLiquidViewModel(NutritionService nutritionService)
        {
            _nutritionService = nutritionService;
            Title = "Adicionar Líquido";

            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        public void Initialize(int diaryId, LiquidDTO? liquid = null)
        {
            DiaryId = diaryId;
            if (liquid != null)
            {
                _liquidId = liquid.Id;
                IsEditing = true;
                Name = liquid.Name;
                QuantityMlText = liquid.QuantityMl.ToString();
                CaloriesPerMlText = liquid.CaloriesPerMl.ToString();
            }
            else
            {
                _liquidId = 0;
                IsEditing = false;
                Name = string.Empty;
                QuantityMlText = string.Empty;
                CaloriesPerMlText = "0";
            }
        }

        private async Task SaveAsync()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(Name))
                return;

            if (!int.TryParse(_quantityMlText, out var qty) || qty <= 0)
            {
                await Application.Current!.MainPage!.DisplayAlert("Atenção", "Informe uma quantidade válida em ml.", "OK");
                return;
            }

            if (!int.TryParse(_caloriesPerMlText, out var cal) || cal < 0)
                cal = 0;

            IsBusy = true;
            try
            {
                bool success;
                if (_isEditing)
                {
                    var dto = new UpdateLiquidDTO(_liquidId, Name, qty, cal);
                    success = await _nutritionService.UpdateLiquidAsync(_liquidId, dto);
                }
                else
                {
                    var dto = new CreateLiquidDTO(DiaryId, Name, qty, cal);
                    success = await _nutritionService.CreateLiquidAsync(dto);
                }

                if (success)
                    OnSaved?.Invoke(this, EventArgs.Empty);
                else
                    await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível salvar o líquido.", "OK");
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
