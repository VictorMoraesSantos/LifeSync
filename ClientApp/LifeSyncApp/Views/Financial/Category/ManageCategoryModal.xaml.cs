using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial;

[QueryProperty(nameof(Category), "Category")]
public partial class ManageCategoryModal : ContentPage
{
    private readonly ManageCategoryViewModel _viewModel;
    private readonly CategoriesViewModel _categoriesViewModel;

    public CategoryDTO? Category { get; set; }

    public ManageCategoryModal(ManageCategoryViewModel viewModel, CategoriesViewModel categoriesViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _categoriesViewModel = categoriesViewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.OnSaved += OnSaved;
        _viewModel.OnCancelled += OnCancelled;

        _viewModel.Initialize(Category);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        _categoriesViewModel.InvalidateCategoriesCache();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
