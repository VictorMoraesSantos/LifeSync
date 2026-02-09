using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class TaskItemPage : ContentPage
{
    private readonly TaskItemsViewModel _viewModel;
    private bool _isLoaded;

    public TaskItemPage(TaskItemsViewModel vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_isLoaded)
        {
            _isLoaded = true;
            await _viewModel.LoadTasksAsync();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.IsManageTaskModalOpen = false;
        _viewModel.IsFilterTaskModalOpen = false;
    }
}
