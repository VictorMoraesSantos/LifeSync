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

		if (BindingContext is LifeSyncApp.ViewModels.TaskManager.TaskItemsViewModel vm)
			vm.DueDatePicker = DueDatePicker;
	}
}