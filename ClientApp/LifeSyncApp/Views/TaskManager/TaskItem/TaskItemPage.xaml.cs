using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class TaskItemPage : ContentPage
{
    private readonly TaskItemsViewModel _viewModel;
    private CancellationTokenSource? _skeletonAnimationCts;

    public TaskItemPage(TaskItemsViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.IsLoadingTasks)
            StartSkeletonAnimation();

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;

        await _viewModel.LoadTasksAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        StopSkeletonAnimation();

        _viewModel.IsManageTaskModalOpen = false;
        _viewModel.IsFilterTaskModalOpen = false;
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskItemsViewModel.IsLoadingTasks))
        {
            if (_viewModel.IsLoadingTasks)
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
