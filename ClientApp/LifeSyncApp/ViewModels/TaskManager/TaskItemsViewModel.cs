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
        public ICommand GoToLabelsCommand { get; }
        public ICommand OpenCreateTaskModalCommand { get; }
        public ICommand CloseCreateTaskModalCommand { get; }
        public ICommand CreateTaskCommand { get; }
        public ICommand SetPriorityCommand { get; }
        public ICommand FocusDueDatePickerCommand { get; }

        public DatePicker? DueDatePicker { get; set; }

        private bool _isCreateTaskModalOpen;
        public bool IsCreateTaskModalOpen
        {
            get => _isCreateTaskModalOpen;
            set
            {
                if (_isCreateTaskModalOpen != value)
                {
                    _isCreateTaskModalOpen = value;
                    OnPropertyChanged(nameof(IsCreateTaskModalOpen));
                }
            }
        }

        private string? _newTaskTitle;
        public string? NewTaskTitle
        {
            get => _newTaskTitle;
            set
            {
                if (_newTaskTitle != value)
                {
                    _newTaskTitle = value;
                    OnPropertyChanged(nameof(NewTaskTitle));
                    OnPropertyChanged(nameof(CanCreateTask));
                }
            }
        }

        private string? _newTaskDescription;
        public string? NewTaskDescription
        {
            get => _newTaskDescription;
            set
            {
                if (_newTaskDescription != value)
                {
                    _newTaskDescription = value;
                    OnPropertyChanged(nameof(NewTaskDescription));
                }
            }
        }

        public ObservableCollection<Priority> PriorityOptions { get; } = new(Enum.GetValues<Priority>());

        private Priority _newTaskPriority = Priority.Medium;
        public Priority NewTaskPriority
        {
            get => _newTaskPriority;
            set
            {
                if (_newTaskPriority != value)
                {
                    _newTaskPriority = value;
                    OnPropertyChanged(nameof(NewTaskPriority));
                }
            }
        }

        private DateTime _newTaskDueDate = DateTime.Today;
        public DateTime NewTaskDueDate
        {
            get => _newTaskDueDate;
            set
            {
                if (_newTaskDueDate != value)
                {
                    _newTaskDueDate = value;
                    OnPropertyChanged(nameof(NewTaskDueDate));
                }
            }

        }

        public bool CanCreateTask => !string.IsNullOrWhiteSpace(NewTaskTitle);

        public TaskItemsViewModel(TaskItemService taskItemService)
        {
            _taskItemService = taskItemService;

            ToggleStatusCommand = new Command<TaskItem>(async (task) => await ToggleStatusAsync(task));
            OpenCreateTaskModalCommand = new Command(OpenCreateTaskModal);
            CloseCreateTaskModalCommand = new Command(CloseCreateTaskModal);
            CreateTaskCommand = new Command(async () => await CreateTaskAsync());
            SetPriorityCommand = new Command<Priority>(p => NewTaskPriority = p);
            FocusDueDatePickerCommand = new Command(() => DueDatePicker?.Focus());
            DueDatePicker = new DatePicker();
        }

        private void OpenCreateTaskModal()
        {
            NewTaskTitle = null;
            NewTaskDescription = null;
            NewTaskPriority = Priority.Medium;
            NewTaskDueDate = DateTime.Today;
            IsCreateTaskModalOpen = true;
        }

        private void CloseCreateTaskModal() => IsCreateTaskModalOpen = false;

        private async Task CreateTaskAsync()
        {
            if (!CanCreateTask || IsBusy)
                return;

            try
            {
                IsBusy = true;

                const int userId = 22;

                var dto = new CreateTaskItemDTO(
                    Title: NewTaskTitle!,
                    Description: NewTaskDescription ?? string.Empty,
                    Priority: NewTaskPriority,
                    DueDate: DateOnly.FromDateTime(NewTaskDueDate),
                    UserId: userId,
                    TaskLabelsId: null);

                var newId = await _taskItemService.CreateTaskItemAsync(dto);

                var created = new TaskItem
                {
                    Id = newId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Priority = dto.Priority,
                    DueDate = dto.DueDate,
                    Status = Status.Pending,
                    UserId = dto.UserId,
                    CreatedAt = DateTime.UtcNow
                };

                InsertTaskIntoGroups(created);

                IsCreateTaskModalOpen = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void InsertTaskIntoGroups(TaskItem task)
        {
            _taskItems.Add(task);

            var existingGroup = _groupedTasks.FirstOrDefault(g => g.DueDate == task.DueDate);
            if (existingGroup is null)
            {
                var newGroup = new TaskGroup(task.DueDate, new[] { task });

                var insertIndex = 0;
                while (insertIndex < _groupedTasks.Count && _groupedTasks[insertIndex].DueDate < task.DueDate)
                    insertIndex++;

                _groupedTasks.Insert(insertIndex, newGroup);
                return;
            }

            existingGroup.Add(task);

            var index = _groupedTasks.IndexOf(existingGroup);
            _groupedTasks[index] = new TaskGroup(existingGroup.DueDate, existingGroup);
        }

        public async Task LoadTasksAsync()
        {
            var query = new TaskItemFilterDTO(UserId: 22);
            var tasks = await _taskItemService.SearchTaskItemAsync(query);

            _taskItems.Clear();
            _groupedTasks.Clear();

            foreach (var task in tasks)
                _taskItems.Add(task);

            _groupedTasks.Clear();

            var grouped = tasks.ToList()
                .OrderBy(t => t.DueDate)
                .GroupBy(t => t.DueDate)
                .Select(g => new TaskGroup(g.Key, g));

            foreach (var group in grouped)
                _groupedTasks.Add(group);
        }

        public async Task ToggleStatusAsync(TaskItem task)
        {
            if (task is null)
                return;

            var updatedStatus = task.Status switch
            {
                Status.Pending => Status.InProgress,
                Status.InProgress => Status.Completed,
                Status.Completed => Status.Pending,
                _ => Status.Pending
            };

            task.Status = updatedStatus;

            var updatedItem = new UpdateTaskItemDTO(task.Title, task.Description, updatedStatus, task.Priority, task.DueDate);
            await _taskItemService.UpdateTaskItemAsync(task.Id, updatedItem);
        }
    }
}
