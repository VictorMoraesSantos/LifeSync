using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(Meal), "Meal")]
public partial class MealDetailPage : ContentPage
{
    private readonly MealDetailViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    public MealDTO? Meal { get; set; }

    public MealDetailPage(MealDetailViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.MealDeleted += OnMealDeleted;

        if (Meal != null)
        {
            // New navigation — set the meal from query parameter
            _viewModel.Meal = Meal;
            Meal = null; // Clear so back-navigation doesn't re-set stale data
        }
        else if (_viewModel.Meal != null)
        {
            // Returning from sub-page — refresh current meal from API
            await _viewModel.RefreshMealAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.MealDeleted -= OnMealDeleted;
        _nutritionViewModel.InvalidateDataCache();
    }

    private void OnMealDeleted(object? sender, EventArgs e)
    {
        _nutritionViewModel.InvalidateDataCache();
    }
}
