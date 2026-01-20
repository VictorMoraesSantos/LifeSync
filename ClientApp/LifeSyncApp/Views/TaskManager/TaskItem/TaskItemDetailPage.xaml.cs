using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

[QueryProperty(nameof(TaskId), "taskId")]
public partial class TaskItemDetailPage : ContentPage
{
    private TaskItemsViewModel _viewModel;

    private int _taskId;
    public int TaskId
    {
        get => _taskId;
        set => _taskId = value;
    }

    public TaskItemDetailPage(TaskItemsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (TaskId > 0)
            await _viewModel.LoadTaskDetailAsync(TaskId);
    }
}
