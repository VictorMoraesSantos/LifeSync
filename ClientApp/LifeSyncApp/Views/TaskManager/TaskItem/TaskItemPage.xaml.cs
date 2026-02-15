using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class TaskItemPage : ContentPage
{
    private readonly TaskItemsViewModel _viewModel;

    public TaskItemPage(TaskItemsViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // LoadTasksAsync now uses smart caching - it will only fetch from API if cache expired
        await _viewModel.LoadTasksAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.IsManageTaskModalOpen = false;
        _viewModel.IsFilterTaskModalOpen = false;
    }
}
