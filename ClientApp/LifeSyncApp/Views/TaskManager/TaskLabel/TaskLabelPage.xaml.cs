using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskLabel;

public partial class TaskLabelPage : ContentPage
{
    private readonly TaskLabelViewModel _taskLabelViewModel;

    public TaskLabelPage(TaskLabelViewModel taskLabelViewModel)
    {
        InitializeComponent();
        BindingContext = taskLabelViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is not TaskLabelViewModel viewModel)
            return;

        await viewModel.LoadLabelsAsync();
    }
}