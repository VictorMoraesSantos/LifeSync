using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.ViewModels.Financial;
using LifeSyncApp.ViewModels.Financial.Category;

namespace LifeSyncApp.Views.Financial;

[QueryProperty(nameof(Category), "Category")]
public partial class ManageCategoryModal : ContentPage
{
    private readonly ManageCategoryViewModel _viewModel;
    private readonly CategoriesViewModel _categoriesViewModel;
    private readonly FinancialViewModel _financialViewModel;

    public CategoryDTO? Category { get; set; }

    public ManageCategoryModal(ManageCategoryViewModel viewModel, CategoriesViewModel categoriesViewModel, FinancialViewModel financialViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _categoriesViewModel = categoriesViewModel;
        _financialViewModel = financialViewModel;
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
        _financialViewModel.InvalidateDataCache();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
