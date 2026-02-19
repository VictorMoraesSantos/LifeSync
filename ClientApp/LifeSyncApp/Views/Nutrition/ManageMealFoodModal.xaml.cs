using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(MealId), "MealId")]
public partial class ManageMealFoodModal : ContentPage
{
    private readonly ManageMealFoodViewModel _viewModel;
    private readonly MealDetailViewModel _mealDetailViewModel;

    public int MealId { get; set; }

    public ManageMealFoodModal(ManageMealFoodViewModel viewModel, MealDetailViewModel mealDetailViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _mealDetailViewModel = mealDetailViewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnSaved += OnSaved;
        _viewModel.OnCancelled += OnCancelled;
        _viewModel.Initialize(MealId);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
        await _mealDetailViewModel.RefreshMealAsync();
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
