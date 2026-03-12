using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskLabel;

public partial class TaskLabelPage : ContentPage
{
    private readonly TaskLabelViewModel _viewModel;
    private CancellationTokenSource? _skeletonAnimationCts;

    public TaskLabelPage(TaskLabelViewModel taskLabelViewModel)
    {
        InitializeComponent();
        _viewModel = taskLabelViewModel;
        BindingContext = taskLabelViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.IsLoadingLabels)
            StartSkeletonAnimation();

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        await _viewModel.LoadLabelsAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        StopSkeletonAnimation();

        _viewModel.SelectedLabel = null;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskLabelViewModel.IsLoadingLabels))
        {
            if (_viewModel.IsLoadingLabels)
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
