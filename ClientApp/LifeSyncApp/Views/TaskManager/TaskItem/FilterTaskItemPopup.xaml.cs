using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class FilterTaskItemPopup : ContentView
{
    private readonly TaskItemsViewModel _taskItemsViewModel;
    public FilterTaskItemPopup()
    {
        InitializeComponent();
    }

    public FilterTaskItemPopup(TaskItemsViewModel taskItemsViewModel) : this()
    {
        _taskItemsViewModel = taskItemsViewModel;
        BindingContext = _taskItemsViewModel;
    }
}