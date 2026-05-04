using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.Models.Financial.Category;
using LifeSyncApp.Helpers;
using LifeSyncApp.Services.Financial;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;

namespace LifeSyncApp.ViewModels.Financial.Category
{
    public partial class CategoriesViewModel : BaseViewModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IUserSession _userSession;

        [ObservableProperty]
        private bool _isLoadingCategories;

        private DateTime? _lastCategoriesRefresh;

        public ObservableCollection<CategoryDTO> Categories { get; } = new();

        public CategoriesViewModel(ICategoryService categoryService, IUserSession userSession)
        {
            _categoryService = categoryService;
            _userSession = userSession;
            Title = "Categorias";
        }

        public async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
        }

        public async Task LoadCategoriesAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCacheExpired(_lastCategoriesRefresh) && Categories.Any()) return;

            if (IsLoadingCategories) return;

            try
            {
                IsLoadingCategories = true;
                IsBusy = true;

                var categories = await _categoryService.GetCategoriesByUserIdAsync(_userSession.UserId);

                Categories.ReplaceAll(categories);

                _lastCategoriesRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert(
                    "Erro",
                    "Não foi possível exibir as categorias.",
                    "OK");
            }
            finally
            {
                IsLoadingCategories = false;
                IsBusy = false;
            }
        }

        public void InvalidateCategoriesCache()
        {
            _lastCategoriesRefresh = null;
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task AddCategoryAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.ManageCategoryModal);
        }

        [RelayCommand]
        private async Task EditCategoryAsync(CategoryDTO category)
        {
            var parameters = new Dictionary<string, object>
            {
                { "Category", category }
            };

            await Shell.Current.GoToAsync(AppRoutes.ManageCategoryModal, parameters);
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync(CategoryDTO category)
        {
            if (category == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Confirmar Exclusão",
                $"Tem certeza que deseja excluir a categoria '{category.Name}'?",
                "Sim",
                "Não");

            if (!confirm) return;

            IsBusy = true;

            try
            {
                var (success, error) = await _categoryService.DeleteCategoryAsync(category.Id);
                if (success)
                {
                    InvalidateCategoriesCache();
                    Categories.Remove(category);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Erro", error ?? "Não foi possível excluir a categoria.", "OK");
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
    }
}
