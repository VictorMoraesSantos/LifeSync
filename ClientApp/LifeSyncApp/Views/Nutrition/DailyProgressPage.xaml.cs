using LifeSyncApp.DTOs.Nutrition.DailyProgress;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

public partial class DailyProgressPage : ContentPage, IQueryAttributable
{
    private readonly DailyProgressViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;

    private DailyProgressDTO? _dailyProgress;

    public DailyProgressPage(DailyProgressViewModel viewModel, NutritionViewModel nutritionViewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _nutritionViewModel = nutritionViewModel;
        BindingContext = _viewModel;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("DailyProgress", out var value) && value is DailyProgressDTO dto)
            _dailyProgress = dto;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Initialize(_dailyProgress, _nutritionViewModel.CaloriesConsumed, _nutritionViewModel.LiquidsConsumedMl);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _nutritionViewModel.InvalidateDataCache();
    }
}
