using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class CreateTaskItemPopup : ContentView
{
    public CreateTaskItemPopup()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext is TaskItemsViewModel vm)
            vm.DueDatePicker = DueDatePicker;
    }
}