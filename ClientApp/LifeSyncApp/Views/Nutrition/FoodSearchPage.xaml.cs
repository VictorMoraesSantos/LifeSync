using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(MealId), "MealId")]
public partial class FoodSearchPage : ContentPage
{
    private readonly FoodSearchViewModel _viewModel;

    public int MealId { get; set; }

    public FoodSearchPage(FoodSearchViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
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
        MessagingCenter.Send(this, "MealFoodChanged");
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
