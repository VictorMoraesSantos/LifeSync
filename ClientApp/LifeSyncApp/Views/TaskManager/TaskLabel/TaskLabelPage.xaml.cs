using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskLabel;

public partial class TaskLabelPage : ContentPage
{
    private readonly TaskLabelViewModel _viewModel;

    public TaskLabelPage(TaskLabelViewModel taskLabelViewModel)
    {
        InitializeComponent();
        _viewModel = taskLabelViewModel;
        BindingContext = taskLabelViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // LoadLabelsAsync now uses smart caching - it will only fetch from API if cache expired
        await _viewModel.LoadLabelsAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.SelectedLabel = null;
    }
}
