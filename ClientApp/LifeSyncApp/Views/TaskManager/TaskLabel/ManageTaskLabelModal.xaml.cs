using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskLabel;

public partial class ManageTaskLabelModal : ContentPage
{
    private readonly TaskLabelViewModel _viewModel;

    public ManageTaskLabelModal(TaskLabelViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
    }
}
