using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(DailyProgress), "DailyProgress")]
public partial class ManageGoalModal : ContentPage
{
    private readonly ManageGoalViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    public DailyProgressDTO? DailyProgress { get; set; }

    public ManageGoalModal(ManageGoalViewModel viewModel, NutritionViewModel nutritionViewModel)
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
        if (DailyProgress != null)
            _viewModel.Initialize(DailyProgress);
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
        _nutritionViewModel.InvalidateDataCache();
        await _nutritionViewModel.LoadDataAsync(forceRefresh: true);
    }

    private async void OnCancelled(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
