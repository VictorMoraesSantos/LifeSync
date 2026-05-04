using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.Services.UserSession;

namespace LifeSyncApp.ViewModels.Financial.Category
{
    public partial class ManageCategoryViewModel : BaseViewModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserSession _userSession;
        private CategoryDTO? _category;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string _name = string.Empty;

        [ObservableProperty]
        private string _categoryDescription = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ActionButtonText))]
        private bool _isEditing;

        public string ActionButtonText => IsEditing ? "Salvar" : "Criar";

        public event EventHandler? OnSaved;
        public event EventHandler? OnCancelled;

        public ManageCategoryViewModel(ICategoryService categoryService, IUserSession userSession)
        {
            _categoryService = categoryService;
            _userSession = userSession;
            Title = "Nova Categoria";
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
                Name = string.Empty;
                CategoryDescription = string.Empty;
            }
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveAsync()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                if (IsEditing && _category != null)
                {
                    var dto = new UpdateCategoryDTO(_category.Id, Name, CategoryDescription);
                    var (success, error) = await _categoryService.UpdateCategoryAsync(_category.Id, dto);
                    if (success)
                        OnSaved?.Invoke(this, EventArgs.Empty);
                    else
                        await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível atualizar a categoria.", "OK");
                }
                else
                {
                    var dto = new CreateCategoryDTO(_userSession.UserId, Name, CategoryDescription);
                    var (id, error) = await _categoryService.CreateCategoryAsync(dto);
                    if (id.HasValue)
                        OnSaved?.Invoke(this, EventArgs.Empty);
                    else
                        await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível criar a categoria.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            OnCancelled?.Invoke(this, EventArgs.Empty);
        }
    }
}
