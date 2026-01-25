using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class TaskItemPage : ContentPage
{
    public TaskItemPage(TaskItemsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is not TaskItemsViewModel viewModel)
            return;

        await viewModel.LoadTasksAsync();
    }

}