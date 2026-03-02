using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

[QueryProperty(nameof(DailyProgress), "DailyProgress")]
public partial class DailyProgressPage : ContentPage
{
    private readonly DailyProgressViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    public DailyProgressDTO? DailyProgress { get; set; }

    public DailyProgressPage(DailyProgressViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initialize(DailyProgress);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _nutritionViewModel.InvalidateDataCache();
    }
}
