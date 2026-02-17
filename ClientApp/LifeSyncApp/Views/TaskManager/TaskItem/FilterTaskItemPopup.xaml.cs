using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class FilterTaskItemPopup : ContentPage
{
    private readonly TaskItemsViewModel _viewModel;

    public FilterTaskItemPopup(TaskItemsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

}