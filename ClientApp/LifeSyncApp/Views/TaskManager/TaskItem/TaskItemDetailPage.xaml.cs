using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

[QueryProperty(nameof(TaskId), "taskId")]
public partial class TaskItemDetailPage : ContentPage
{
    private readonly TaskItemsViewModel _viewModel;
    private ManageTaskItemPopup? _managePopup;

    private int _taskId;
    public int TaskId
    {
        get => _taskId;
        set => _taskId = value;
    }

    public TaskItemDetailPage(TaskItemsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (TaskId > 0)
            await _viewModel.LoadTaskDetailAsync(TaskId);
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskItemsViewModel.IsManageTaskModalOpen))
        {
            if (_viewModel.IsManageTaskModalOpen && _managePopup is null)
            {
                _managePopup = new ManageTaskItemPopup { BindingContext = _viewModel };
                ManageTaskOverlay.Add(_managePopup);
            }
            else if (!_viewModel.IsManageTaskModalOpen && _managePopup is not null)
            {
                ManageTaskOverlay.Remove(_managePopup);
                _managePopup = null;
            }
        }
    }
}
