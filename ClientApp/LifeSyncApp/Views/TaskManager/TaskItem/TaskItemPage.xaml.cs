using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class TaskItemPage : ContentPage
{
    private readonly TaskItemsViewModel _viewModel;

    public TaskItemPage(TaskItemsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TaskItemsViewModel viewModel)
        {
            await viewModel.LoadTasksAsync();
        }
    }
}