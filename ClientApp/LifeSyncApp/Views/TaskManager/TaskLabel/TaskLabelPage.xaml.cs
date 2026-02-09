using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskLabel;

public partial class TaskLabelPage : ContentPage
{
    private readonly TaskLabelViewModel _viewModel;
    private bool _isLoaded;

    public TaskLabelPage(TaskLabelViewModel taskLabelViewModel)
    {
        InitializeComponent();
        _viewModel = taskLabelViewModel;
        BindingContext = taskLabelViewModel;

        _ = Task.Run(async () =>
        {
            if (!_isLoaded)
            {
                await _viewModel.LoadLabelsAsync();
                _isLoaded = true;
            }
        });
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_isLoaded && !_viewModel.IsBusy)
        {
            Task.Run(async () =>
            {
                await _viewModel.LoadLabelsAsync();
                _isLoaded = true;
            });
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.SelectedLabel = null;
    }
}
