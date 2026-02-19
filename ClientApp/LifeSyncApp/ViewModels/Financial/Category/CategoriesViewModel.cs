using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.Services.Financial;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.Financial
{
    public class CategoriesViewModel : BaseViewModel
    {
        private readonly CategoryService _categoryService;
        private int _userId = 1; // TODO: Obter do contexto de autenticação

        private bool _isLoadingCategories;
        private DateTime? _lastCategoriesRefresh;
        private const int CacheExpirationMinutes = 5;

        public ObservableCollection<CategoryDTO> Categories { get; } = new();

        public ICommand GoBackCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }

        public CategoriesViewModel(CategoryService categoryService)
        {
            _categoryService = categoryService;
            Title = "Categorias";

            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            AddCategoryCommand = new Command(async () => await AddCategoryAsync());
            EditCategoryCommand = new Command<CategoryDTO>(async (category) => await EditCategoryAsync(category));
            DeleteCategoryCommand = new Command<CategoryDTO>(async (category) => await DeleteCategoryAsync(category));
        }

        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task LoadCategoriesAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCategoriesCacheExpired() && Categories.Any()) return;

            if (_isLoadingCategories) return;

            try
            {
                _isLoadingCategories = true;
                IsBusy = true;

                var categories = await _categoryService.GetCategoriesByUserIdAsync(_userId);

                Categories.Clear();
                foreach (var category in categories) Categories.Add(category);

                _lastCategoriesRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Erro",
                    "Não foi possível exibir as categorias.",
                    "OK");
            }
            finally
            {
                _isLoadingCategories = false;
                IsBusy = false;
            }
        }

        private bool IsCategoriesCacheExpired()
        {
            if (_lastCategoriesRefresh == null) return true;

            return (DateTime.Now - _lastCategoriesRefresh.Value).TotalMinutes >= CacheExpirationMinutes;
        }

        public void InvalidateCategoriesCache()
        {
            _lastCategoriesRefresh = null;
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
            if (category == null) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirmar Exclusão",
                $"Tem certeza que deseja excluir a categoria '{category.Name}'?",
                "Sim",
                "Não");

            if (!confirm) return;

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
