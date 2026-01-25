using LifeSyncApp.ViewModels.TaskManager;

namespace LifeSyncApp.Views.TaskManager.TaskItem;

public partial class ManageTaskItemPopup : ContentView
{
    public ManageTaskItemPopup()
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