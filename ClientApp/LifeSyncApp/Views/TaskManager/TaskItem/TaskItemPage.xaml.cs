using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class TaskItemPage : ContentPage
{
    private readonly TaskItemsViewModel _viewModel;
    private bool _loaded;

    public TaskItemPage(TaskItemsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_loaded)
            return;

        _loaded = true;

        if (BindingContext is TaskItemsViewModel viewModel)
            await viewModel.LoadTasksAsync();
    }
}