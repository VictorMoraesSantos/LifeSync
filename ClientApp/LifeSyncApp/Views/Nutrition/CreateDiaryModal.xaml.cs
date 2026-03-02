using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

public partial class CreateDiaryModal : ContentPage
{
    private readonly CreateDiaryViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    public CreateDiaryModal(CreateDiaryViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnCreated += OnCreated;
        _viewModel.OnCancelled += OnCancelled;
        _viewModel.Initialize();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.OnCreated -= OnCreated;
        _viewModel.OnCancelled -= OnCancelled;
    }

    private async void OnCreated(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
        _nutritionViewModel.InvalidateDataCache();
        await _nutritionViewModel.LoadDataAsync(forceRefresh: true);
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
