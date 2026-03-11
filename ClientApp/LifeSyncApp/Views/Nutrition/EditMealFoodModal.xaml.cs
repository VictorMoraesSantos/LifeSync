using CommunityToolkit.Mvvm.Messaging;
using LifeSyncApp.DTOs.Nutrition.MealFood;
using LifeSyncApp.Messages;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(MealId), "MealId")]
[QueryProperty(nameof(MealFood), "MealFood")]
public partial class EditMealFoodModal : ContentPage
{
    private readonly EditMealFoodViewModel _viewModel;

    public int MealId { get; set; }
    public MealFoodDTO? MealFood { get; set; }

    public EditMealFoodModal(EditMealFoodViewModel viewModel)
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
        _viewModel.Initialize(MealId, MealFood);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnSaved -= OnSaved;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnSaved(object? sender, EventArgs e)
    {
        WeakReferenceMessenger.Default.Send(new MealFoodChangedMessage());
        await Shell.Current.GoToAsync("..");
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
