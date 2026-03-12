using LifeSyncApp.ViewModels.Nutrition;

namespace LifeSyncApp.Views.Nutrition;

public partial class NutritionPage : ContentPage
{
    private readonly NutritionViewModel _viewModel;
    private CancellationTokenSource? _skeletonAnimationCts;

    public NutritionPage(NutritionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.IsLoadingData)
            StartSkeletonAnimation();

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        await _viewModel.LoadDataAsync(forceRefresh: true);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        StopSkeletonAnimation();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NutritionViewModel.IsLoadingData))
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
