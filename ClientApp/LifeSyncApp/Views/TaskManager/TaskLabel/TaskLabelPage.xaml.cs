using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskLabel;

public partial class TaskLabelPage : ContentPage
{
    private bool _isLoaded;

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

        if (!_isLoaded)
        {
            await viewModel.LoadLabelsAsync();
            _isLoaded = true;
        }
    }
}
