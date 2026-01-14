using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Models.TaskManager.Enums;
using LifeSyncApp.Services.TaskManager.Implementation;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public class TaskItemsViewModel : BaseViewModel
    {
        private readonly TaskItemService _taskItemService;
        private readonly ObservableCollection<TaskItem> _taskItems = new();

        public ObservableCollection<TaskItem> TaskItems => _taskItems;
        public ICommand LoadTasksCommand { get; }
        public ICommand ToggleStatusCommand { get; }

        public TaskItemsViewModel(TaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
            LoadTasksCommand = new Command(async () => await LoadTasksAsync());
            ToggleStatusCommand = new Command<TaskItem>(async (task) => await ToggleStatusAsync(task));
        }

        public async Task LoadTasksAsync()
        {
            var query = new TaskItemFilterDTO(UserId: 22);
            var tasks = await _taskItemService.SearchTaskItemAsync(query);
            _taskItems.Clear();
            foreach (var task in tasks)
            {
                _taskItems.Add(task);
            }
        }
        public async Task ToggleStatusAsync(TaskItem task)
        {
            var index = _taskItems.IndexOf(task);
            if (index < 0)
                return;

            var updatedStatus = task.Status switch
            {
                Status.Pending => Status.InProgress,
                Status.InProgress => Status.Completed,
                Status.Completed => Status.Pending,
                _ => Status.Pending
            };

            var updatedItem = new UpdateTaskItemDTO(
                task.Title,
                task.Description,
                updatedStatus,
                task.Priority,
                task.DueDate);


            await _taskItemService.UpdateTaskItemAsync(task.Id, updatedItem);
            task = await _taskItemService.GetTaskItemAsync(task.Id);

            _taskItems.RemoveAt(index);
            _taskItems.Insert(index, task);
        }
    }
}
