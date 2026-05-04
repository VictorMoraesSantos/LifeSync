using LifeSyncApp.Models.Nutrition.Meal;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(DiaryId), "DiaryId")]
[QueryProperty(nameof(Meal), "Meal")]
public partial class ManageMealModal : ContentPage
{
    private readonly ManageMealViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    public int DiaryId { get; set; }
    public MealDTO? Meal { get; set; }

    public ManageMealModal(ManageMealViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnSaved += OnSaved;
        _viewModel.OnCancelled += OnCancelled;
        _viewModel.Initialize(DiaryId, Meal);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        _nutritionViewModel.InvalidateDataCache();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
