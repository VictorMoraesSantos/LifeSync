using LifeSyncApp.ViewModels.Financial.Category;

namespace LifeSyncApp.Views.Financial;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesViewModel _viewModel;
    private CancellationTokenSource? _skeletonAnimationCts;

    public CategoriesPage(CategoriesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.IsLoadingCategories)
            StartSkeletonAnimation();

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        await _viewModel.InitializeAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        StopSkeletonAnimation();
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CategoriesViewModel.IsLoadingCategories))
        {
            if (_viewModel.IsLoadingCategories)
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
