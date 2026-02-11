using System.Collections.ObjectModel;
using System.Windows.Input;
using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.Services.Financial;

namespace LifeSyncApp.ViewModels.Financial
{
    public class CategoriesViewModel : ViewModels.BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        public ObservableCollection<CategoryDTO> Categories { get; } = new();

        public ICommand LoadCategoriesCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand RefreshCommand { get; }

        public CategoriesViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            Title = "Categorias";

            LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
            AddCategoryCommand = new Command(async () => await AddCategoryAsync());
            EditCategoryCommand = new Command<CategoryDTO>(async (category) => await EditCategoryAsync(category));
            DeleteCategoryCommand = new Command<CategoryDTO>(async (category) => await DeleteCategoryAsync(category));
            RefreshCommand = new Command(async () => await LoadCategoriesAsync());
        }

        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                System.Diagnostics.Debug.WriteLine("Loading categories...");
                var categories = await _categoryService.GetCategoriesByUserIdAsync(_userId);
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
                System.Diagnostics.Debug.WriteLine($"Loaded {categories.Count} categories");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddCategoryAsync()
        {
            await Shell.Current.GoToAsync("ManageCategoryModal");
        }

        private async Task EditCategoryAsync(CategoryDTO category)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Category", category }
            };
            await Shell.Current.GoToAsync("ManageCategoryModal", parameters);
        }

        private async Task DeleteCategoryAsync(CategoryDTO category)
        {
            if (category == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirmar Exclusão",
                $"Tem certeza que deseja excluir a categoria '{category.Name}'?",
                "Sim",
                "Não");

            if (!confirm)
                return;

            IsBusy = true;

            try
            {
                var success = await _categoryService.DeleteCategoryAsync(category.Id);
                if (success)
                {
                    Categories.Remove(category);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting category: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert(
                    "Erro",
                    "Não foi possível excluir a categoria.",
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
