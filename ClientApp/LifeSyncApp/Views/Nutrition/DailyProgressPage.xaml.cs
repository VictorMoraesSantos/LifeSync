using LifeSyncApp.Models.Nutrition.DailyProgress;
using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

public partial class DailyProgressPage : ContentPage, IQueryAttributable
{
    private readonly DailyProgressViewModel _viewModel;
    private readonly NutritionViewModel _nutritionViewModel;
    private CancellationTokenSource? _skeletonAnimationCts;

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

        if (_viewModel.IsLoadingData)
            StartSkeletonAnimation();

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        _viewModel.Initialize(_dailyProgress, _nutritionViewModel.CaloriesConsumed, _nutritionViewModel.LiquidsConsumedMl);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _nutritionViewModel.InvalidateDataCache();
        StopSkeletonAnimation();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DailyProgressViewModel.IsLoadingData))
        {
            if (_viewModel.IsLoadingData)
                StartSkeletonAnimation();
            else
                StopSkeletonAnimation();
        }
    }

    private void StartSkeletonAnimation()
    {
        StopSkeletonAnimation();
        _skeletonAnimationCts = new CancellationTokenSource();
        var token = _skeletonAnimationCts.Token;

        Dispatcher.Dispatch(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await SkeletonContainer.FadeTo(0.4, 800, Easing.SinInOut);
                if (token.IsCancellationRequested) break;
                await SkeletonContainer.FadeTo(1.0, 800, Easing.SinInOut);
            }
        });
    }

    private void StopSkeletonAnimation()
    {
        _skeletonAnimationCts?.Cancel();
        _skeletonAnimationCts?.Dispose();
        _skeletonAnimationCts = null;
    }
}
