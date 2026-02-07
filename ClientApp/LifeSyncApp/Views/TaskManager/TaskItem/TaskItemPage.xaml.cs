using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class TaskItemPage : ContentPage
{
    private bool _isLoaded;
    private ManageTaskItemPopup? _managePopup;
    private FilterTaskItemPopup? _filterPopup;

    public TaskItemPage(TaskItemsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is not TaskItemsViewModel viewModel)
            return;

        if (!_isLoaded)
        {
            await viewModel.LoadTasksAsync();
            _isLoaded = true;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskItemsViewModel.IsManageTaskModalOpen))
        {
            var vm = (TaskItemsViewModel)BindingContext;
            if (vm.IsManageTaskModalOpen && _managePopup is null)
            {
                _managePopup = new ManageTaskItemPopup { BindingContext = vm };
                ManageTaskOverlay.Add(_managePopup);
            }
            else if (!vm.IsManageTaskModalOpen && _managePopup is not null)
            {
                ManageTaskOverlay.Remove(_managePopup);
                _managePopup = null;
            }
        }
        else if (e.PropertyName == nameof(TaskItemsViewModel.IsFilterTaskModalOpen))
        {
            var vm = (TaskItemsViewModel)BindingContext;
            if (vm.IsFilterTaskModalOpen && _filterPopup is null)
            {
                _filterPopup = new FilterTaskItemPopup { BindingContext = vm };
                FilterTaskOverlay.Add(_filterPopup);
            }
            else if (!vm.IsFilterTaskModalOpen && _filterPopup is not null)
            {
                FilterTaskOverlay.Remove(_filterPopup);
                _filterPopup = null;
            }
        }
    }
}
