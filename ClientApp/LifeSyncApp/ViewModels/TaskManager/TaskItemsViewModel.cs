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
        public ICommand ViewTaskDetailCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }

        public DatePicker? DueDatePicker { get; set; }

        private bool _isCreateTaskModalOpen;
        public bool IsCreateTaskModalOpen
        {
            get => _isCreateTaskModalOpen;
            set
            {
                _isCreateTaskModalOpen = value;
                OnPropertyChanged(nameof(IsCreateTaskModalOpen));
            }
        }

        private int? _editingTaskId;
        public int? EditingTaskId
        {
            get => _editingTaskId;
            set
            {
                _editingTaskId = value;
                OnPropertyChanged(nameof(EditingTaskId));
                OnPropertyChanged(nameof(IsEditMode));
                OnPropertyChanged(nameof(ModalTitle));
                OnPropertyChanged(nameof(SaveButtonText));
            }
        }

        public bool IsEditMode => EditingTaskId.HasValue;
        public string ModalTitle => IsEditMode ? "Editar Tarefa" : "Nova Tarefa";
        public string SaveButtonText => IsEditMode ? "Salvar Alterações" : "Criar Tarefa";

        private string? _newTaskTitle;
        public string? NewTaskTitle
        {
            get => _newTaskTitle;
            set
            {
                _newTaskTitle = value;
                OnPropertyChanged(nameof(NewTaskTitle));
                OnPropertyChanged(nameof(CanCreateTask));
            }
        }

        private string? _newTaskDescription;
        public string? NewTaskDescription
        {
            get => _newTaskDescription;
            set
            {
                _newTaskDescription = value;
                OnPropertyChanged(nameof(NewTaskDescription));
            }
        }

        private TaskItem? _selectedTask;
        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public ObservableCollection<Priority> PriorityOptions { get; } = new(Enum.GetValues<Priority>());

        private Priority _newTaskPriority = Priority.Medium;
        public Priority NewTaskPriority
        {
            get => _newTaskPriority;
            set
            {
                _newTaskPriority = value;
                OnPropertyChanged(nameof(NewTaskPriority));
            }
        }

        private DateTime _newTaskDueDate = DateTime.Today;
        public DateTime NewTaskDueDate
        {
            get => _newTaskDueDate;
            set
            {
                _newTaskDueDate = value;
                OnPropertyChanged(nameof(NewTaskDueDate));
            }
        }

        public bool CanCreateTask => !string.IsNullOrWhiteSpace(NewTaskTitle);

        public TaskItemsViewModel(TaskItemService taskItemService)
        {
            _taskItemService = taskItemService;

            ToggleStatusCommand = new Command<TaskItem>(async (task) => await ToggleStatusAsync(task));
            OpenCreateTaskModalCommand = new Command<TaskItem>(OpenCreateTaskModal);
            CloseCreateTaskModalCommand = new Command(CloseCreateTaskModal);
            CreateTaskCommand = new Command(async () => await CreateTaskAsync());
            SetPriorityCommand = new Command<Priority>(p => NewTaskPriority = p);
            FocusDueDatePickerCommand = new Command(() => DueDatePicker?.Focus());
            ViewTaskDetailCommand = new Command<TaskItem>(async (task) => await NavigateToTaskDetailAsync(task));
            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            EditTaskCommand = new Command(async () => await EditTaskAsync());
            DeleteTaskCommand = new Command(async () => await DeleteTaskAsync());
            DueDatePicker = new DatePicker();
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

        private void OpenCreateTaskModal(TaskItem? taskToEdit = null)
        {
            if (taskToEdit != null)
            {
                EditingTaskId = taskToEdit.Id;
                NewTaskTitle = taskToEdit.Title;
                NewTaskDescription = taskToEdit.Description;
                NewTaskPriority = taskToEdit.Priority;
                NewTaskDueDate = taskToEdit.DueDate.ToDateTime(TimeOnly.MinValue);
            }
            else
            {
                EditingTaskId = null;
                NewTaskTitle = null;
                NewTaskDescription = null;
                NewTaskPriority = Priority.Medium;
                NewTaskDueDate = DateTime.Today;
            }

            IsCreateTaskModalOpen = true;
        }

        private void CloseCreateTaskModal() => IsCreateTaskModalOpen = false;

        private async Task NavigateToTaskDetailAsync(TaskItem task)
        {
            if (task == null) return;

            SelectedTask = task;

            await Shell.Current.GoToAsync($"taskdetail?taskId={task.Id}");
        }

        public async Task LoadTaskDetailAsync(int taskId)
        {
            var task = _taskItems.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                SelectedTask = task;
            }
            else
            {
                var query = new TaskItemFilterDTO(UserId: 22);
                var tasks = await _taskItemService.SearchTaskItemAsync(query);
                SelectedTask = tasks.FirstOrDefault(t => t.Id == taskId);
            }
        }

        private async Task EditTaskAsync()
        {
            if (SelectedTask == null) return;

            await Shell.Current.GoToAsync("..");
            OpenCreateTaskModal(SelectedTask);
        }

        private async Task CreateTaskAsync()
        {
            if (!CanCreateTask || IsBusy)
                return;

            try
            {
                IsBusy = true;
                const int userId = 22;

                if (IsEditMode)
                {
                    var existingTask = _taskItems.FirstOrDefault(t => t.Id == EditingTaskId);
                    var currentStatus = existingTask?.Status ?? Status.Pending;
                    var updateDto = new UpdateTaskItemDTO(
                        Title: NewTaskTitle!,
                        Description: NewTaskDescription ?? string.Empty,
                        Status: currentStatus,
                        Priority: NewTaskPriority,
                        DueDate: DateOnly.FromDateTime(NewTaskDueDate)
                    );

                    await _taskItemService.UpdateTaskItemAsync(EditingTaskId!.Value, updateDto);

                    if (existingTask != null)
                    {
                        existingTask.Title = NewTaskTitle!;
                        existingTask.Description = NewTaskDescription ?? string.Empty;
                        existingTask.Priority = NewTaskPriority;
                        existingTask.DueDate = DateOnly.FromDateTime(NewTaskDueDate);

                        await LoadTasksAsync();
                    }
                }
                else
                {
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
                }

                IsCreateTaskModalOpen = false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível salvar a tarefa: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteTaskAsync()
        {
            if (SelectedTask == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Excluir Tarefa",
                "Tem certeza que deseja excluir esta tarefa?",
                "Sim",
                "Não");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                await _taskItemService.DeleteTaskItemAsync(SelectedTask.Id);

                await Shell.Current.DisplayAlert("Sucesso", "Tarefa excluída com sucesso!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível excluir a tarefa: {ex.Message}", "OK");
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
    }
}
