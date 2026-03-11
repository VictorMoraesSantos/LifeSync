using CommunityToolkit.Mvvm.Messaging;
using LifeSyncApp.DTOs.Nutrition.Meal;
using LifeSyncApp.Messages;
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

        WeakReferenceMessenger.Default.Register<MealFoodChangedMessage>(this, async (r, m) =>
        {
            await _viewModel.RefreshMealAsync();
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.MealDeleted += OnMealDeleted;

        if (Meal != null)
        {
            _viewModel.Meal = Meal;
            Meal = null;
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

    ~MealDetailPage()
    {
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}
