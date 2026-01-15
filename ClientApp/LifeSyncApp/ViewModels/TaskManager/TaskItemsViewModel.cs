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
        private readonly ObservableCollection<TaskGroup> _groupedTasks = new();

        public ObservableCollection<TaskItem> TaskItems => _taskItems;
        public ObservableCollection<TaskGroup> GroupedTasks => _groupedTasks;
        public ICommand ToggleStatusCommand { get; }

        public TaskItemsViewModel(TaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
            ToggleStatusCommand = new Command<TaskItem>(async (task) => await ToggleStatusAsync(task));
        }

        public async Task LoadTasksAsync()
        {
            var query = new TaskItemFilterDTO(UserId: 22);
            var tasks = await _taskItemService.SearchTaskItemAsync(query);

            _taskItems.Clear();
            _groupedTasks.Clear();

            foreach (var task in tasks)
            {
                _taskItems.Add(task);
            }

            var grouped = _taskItems
                .OrderBy(t => t.DueDate)
                .GroupBy(t => t.DueDate)
                .Select(g => new TaskGroup(g.Key, g));

            foreach (var group in grouped)
            {
                _groupedTasks.Add(group);
            }
        }

        public async Task ToggleStatusAsync(TaskItem task)
        {
            if (task == null)
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

            var updatedTask = await _taskItemService.GetTaskItemAsync(task.Id);

            var index = _taskItems.IndexOf(task);
            _taskItems[index] = updatedTask;

            var group = _groupedTasks.FirstOrDefault(g => g.Contains(task));
            if (group != null)
            {
                var groupIndex = group.IndexOf(task);
                if (groupIndex >= 0)
                    group[groupIndex] = updatedTask;
            }
        }
    }
}
