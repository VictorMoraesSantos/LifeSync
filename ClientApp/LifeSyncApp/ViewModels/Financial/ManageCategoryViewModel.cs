using System.Windows.Input;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial
{
    public class ManageCategoryViewModel : ViewModels.BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        private string _name = string.Empty;
        private string _description = string.Empty;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string CategoryDescription
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageCategoryViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            Title = "Nova Categoria";

            SaveCommand = new Command(async () => await SaveAsync(), CanSave);
            CancelCommand = new Command(() => OnCancelled?.Invoke(this, EventArgs.Empty));
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        private async Task SaveAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                System.Diagnostics.Debug.WriteLine($"Creating category: {Name}");
                var dto = new CreateCategoryDTO
                {
                    UserId = _userId,
                    Name = Name,
                    Description = string.IsNullOrWhiteSpace(CategoryDescription) ? null : CategoryDescription
                };

                var id = await _categoryService.CreateCategoryAsync(dto);
                if (id.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine($"Category created with ID: {id.Value}");
                    OnSaved?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to create category");
                    await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possível criar a categoria. Verifique sua conexão e tente novamente.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving category: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
