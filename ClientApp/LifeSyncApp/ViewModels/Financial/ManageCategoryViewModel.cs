using System.Windows.Input;
using LifeSyncApp.DTOs.Financial;
using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial
{
    public class ManageCategoryViewModel : ViewModels.BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação
        private CategoryDTO? _category;

        private string _name = string.Empty;
        private string _description = string.Empty;
        private bool _isEditing;

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

        public bool IsEditing
        {
            get => _isEditing;
            private set => SetProperty(ref _isEditing, value);
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

        public void Initialize(CategoryDTO? category = null)
        {
            _category = category;
            IsEditing = category != null;
            Title = IsEditing ? "Editar Categoria" : "Nova Categoria";

            if (IsEditing && _category != null)
            {
                Name = _category.Name;
                CategoryDescription = _category.Description ?? string.Empty;
            }
            else
            {
                // Clear fields for new category
                Name = string.Empty;
                CategoryDescription = string.Empty;
            }
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
                if (IsEditing && _category != null)
                {
                    var dto = new UpdateCategoryDTO(_category.Id, Name, CategoryDescription);
                    var success = await _categoryService.UpdateCategoryAsync(_category.Id, dto);
                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine("Category updated successfully");
                        OnSaved?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to update category");
                        await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível atualizar a categoria.", "OK");
                    }
                }
                else
                {
                    var dto = new CreateCategoryDTO(_userId, Name, CategoryDescription);
                    var id = await _categoryService.CreateCategoryAsync(dto);
                    if (id.HasValue)
                    {
                        System.Diagnostics.Debug.WriteLine($"Category created with ID: {id.Value}");
                        OnSaved?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to create category");
                        await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível criar a categoria.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving category: {ex.Message}");
                await Application.Current!.MainPage!.DisplayAlert("Erro", "Não foi possível salvar a categoria. Verifique sua conexão e tente novamente.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
