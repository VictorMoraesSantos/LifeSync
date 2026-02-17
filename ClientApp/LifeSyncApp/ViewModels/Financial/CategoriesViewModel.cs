using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.Services.Financial;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial
{
    public class CategoriesViewModel : ViewModels.BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        // Cache management
        private bool _isLoadingCategories;
        private DateTime? _lastCategoriesRefresh;
        private const int CacheExpirationMinutes = 5;

        public ObservableCollection<CategoryDTO> Categories { get; } = new();

        public ICommand GoBackCommand { get; }
        public ICommand LoadCategoriesCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand RefreshCommand { get; }

        public CategoriesViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            Title = "Categorias";

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
            AddCategoryCommand = new Command(async () => await AddCategoryAsync());
            EditCategoryCommand = new Command<CategoryDTO>(async (category) => await EditCategoryAsync(category));
            DeleteCategoryCommand = new Command<CategoryDTO>(async (category) => await DeleteCategoryAsync(category));
            RefreshCommand = new Command(async () => await LoadCategoriesAsync(forceRefresh: true));
        }

        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task LoadCategoriesAsync(bool forceRefresh = false)
        {
            // Use cached data if still valid
            if (!forceRefresh && !IsCategoriesCacheExpired() && Categories.Any())
            {
                System.Diagnostics.Debug.WriteLine("📦 Using cached categories (not expired)");
                return;
            }

            // Prevent concurrent loads
            if (_isLoadingCategories)
            {
                System.Diagnostics.Debug.WriteLine("⏳ Categories already loading, skipping duplicate request");
                return;
            }

            try
            {
                _isLoadingCategories = true;
                IsBusy = true;

                System.Diagnostics.Debug.WriteLine($"🔄 Loading categories from API (forceRefresh: {forceRefresh})");
                var categories = await _categoryService.GetCategoriesByUserIdAsync(_userId);

                Categories.Clear();
                foreach (var category in categories)
                    Categories.Add(category);

                _lastCategoriesRefresh = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"✅ Categories loaded ({categories.Count})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
            }
            finally
            {
                _isLoadingCategories = false;
                IsBusy = false;
            }
        }

        private bool IsCategoriesCacheExpired()
        {
            if (_lastCategoriesRefresh == null)
                return true;

            return (DateTime.Now - _lastCategoriesRefresh.Value).TotalMinutes >= CacheExpirationMinutes;
        }

        public void InvalidateCategoriesCache()
        {
            _lastCategoriesRefresh = null;
            System.Diagnostics.Debug.WriteLine("🗑️ Categories cache invalidated");
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
                    InvalidateCategoriesCache();
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
