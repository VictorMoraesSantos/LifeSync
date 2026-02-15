using LifeSyncApp.DTOs.Financial.Category;
using LifeSyncApp.ViewModels.Financial;

namespace LifeSyncApp.Views.Financial;

[QueryProperty(nameof(Category), "Category")]
public partial class ManageCategoryModal : ContentPage
{
    private readonly ManageCategoryViewModel _viewModel;

    public CategoryDTO? Category { get; set; }

    public ManageCategoryModal(ManageCategoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Subscribe to events
        _viewModel.OnSaved += async (sender, args) =>
        {
            await Shell.Current.GoToAsync("..");
        };

        _viewModel.OnCancelled += async (sender, args) =>
        {
            await Shell.Current.GoToAsync("..");
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initialize(Category);
    }
}
