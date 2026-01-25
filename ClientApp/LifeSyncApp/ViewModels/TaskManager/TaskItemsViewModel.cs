using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Models.TaskManager.Enums;
using LifeSyncApp.Services.TaskManager.Implementation;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static LifeSyncApp.ViewModels.TaskManager.FilterTaskItemViewModel;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public class TaskItemsViewModel : BaseViewModel
    {
        private readonly TaskItemService _taskItemService;

        private readonly ObservableCollection<TaskItem> _taskItems = new();
        public ObservableCollection<TaskItem> TaskItems => _taskItems;

        private readonly ObservableCollection<TaskGroup> _groupedTasks = new();
        public ObservableCollection<TaskGroup> GroupedTasks => _groupedTasks;

        public FilterTaskItemViewModel FilterViewModel { get; private set; }

        public DatePicker? DueDatePicker { get; set; }

        private Status? _currentStatusFilter;
        private Priority? _currentPriorityFilter;
        private DateFilterOption? _currentDateFilter = DateFilterOption.All;

        public ICommand ToggleStatusCommand { get; }
        public ICommand GoToLabelsCommand { get; }
        public ICommand OpenManageTaskModalCommand { get; }
        public ICommand CloseManageTaskModalCommand { get; }
        public ICommand ManageTaskCommand { get; }
        public ICommand SetPriorityCommand { get; }
        public ICommand FocusDueDatePickerCommand { get; }
        public ICommand ViewTaskDetailCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand OpenFiltersCommand { get; set; }

        private bool _isManageTaskModalOpen;
        public bool IsManageTaskModalOpen
        {
            get => _isManageTaskModalOpen;
            set
            {
                _isManageTaskModalOpen = value;
                OnPropertyChanged(nameof(IsManageTaskModalOpen));
            }
        }

        private bool _isFilterTaskModalOpen;
        public bool IsFilterTaskModalOpen
        {
            get => _isFilterTaskModalOpen;
            set
            {
                _isFilterTaskModalOpen = value;
                OnPropertyChanged(nameof(IsFilterTaskModalOpen));
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

        public bool CanManageTask => !string.IsNullOrWhiteSpace(NewTaskTitle);
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
                OnPropertyChanged(nameof(CanManageTask));
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

        public TaskItemsViewModel(TaskItemService taskItemService)
        {
            _taskItemService = taskItemService;

            GoToLabelsCommand = new Command(async () => await NavigateToTaskLabelPage());
            ToggleStatusCommand = new Command<TaskItem>(async (task) => await ToggleStatusAsync(task));
            OpenManageTaskModalCommand = new Command<TaskItem>(OpenManageTaskModal);
            CloseManageTaskModalCommand = new Command(() => IsManageTaskModalOpen = false);
            ManageTaskCommand = new Command(async () => await ManageTaskAsync());
            SetPriorityCommand = new Command<Priority>(p => NewTaskPriority = p);
            FocusDueDatePickerCommand = new Command(() => { });
            ViewTaskDetailCommand = new Command<TaskItem>(async (task) => await NavigateToTaskDetailAsync(task));
            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            EditTaskCommand = new Command(async () => await EditTaskAsync());
            DeleteTaskCommand = new Command(async () => await DeleteTaskAsync());
            OpenFiltersCommand = new Command(() => IsFilterTaskModalOpen = true);
            FilterViewModel = new FilterTaskItemViewModel(
                onApplyFilters: (status, priority, dateFilter) =>
                {
                    _ = ApplyFiltersAsync(status, priority, dateFilter);
                },
                onCloseModal: () => IsFilterTaskModalOpen = false
            );
        }

        public async Task LoadTasksAsync()
        {
            try
            {
                var query = new TaskItemFilterDTO(
                    UserId: 22,
                    Status: _currentStatusFilter,
                    Priority: _currentPriorityFilter,
                    DueDate: GetDueDateFromFilter(_currentDateFilter));

                var tasks = await _taskItemService.SearchTaskItemAsync(query);

                _taskItems.Clear();
                _groupedTasks.Clear();

                foreach (var task in tasks)
                    _taskItems.Add(task);

                var grouped = tasks
                    .OrderBy(t => t.DueDate)
                    .GroupBy(t => t.DueDate)
                    .Select(g => new TaskGroup(g.Key, g));

                foreach (var group in grouped)
                    _groupedTasks.Add(group);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar tarefas: {ex.Message}", "OK");
            }
        }

        private async Task ApplyFiltersAsync(Status? status, Priority? priority, DateFilterOption? dateFilter)
        {
            try
            {
                IsBusy = true;

                _currentStatusFilter = status;
                _currentPriorityFilter = priority;
                _currentDateFilter = dateFilter ?? DateFilterOption.All;

                await LoadTasksAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao aplicar filtros: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task NavigateToTaskLabelPage()
        {
            await Shell.Current.GoToAsync("tasklabels");
        }

        private DateOnly? GetDueDateFromFilter(DateFilterOption? filter)
        {
            return filter switch
            {
                DateFilterOption.Today => DateOnly.FromDateTime(DateTime.Today),
                DateFilterOption.ThisWeek => DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                DateFilterOption.ThisMonth => DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                DateFilterOption.All => null,
                _ => null
            };
        }

        public async Task ToggleStatusAsync(TaskItem task)
        {
            if (task is null)
                return;

            try
            {
                var updatedStatus = task.Status switch
                {
                    Status.Pending => Status.InProgress,
                    Status.InProgress => Status.Completed,
                    Status.Completed => Status.Pending,
                    _ => Status.Pending
                };

                task.Status = updatedStatus;

                var updatedItem = new UpdateTaskItemDTO(
                    task.Title,
                    task.Description,
                    updatedStatus,
                    task.Priority,
                    task.DueDate
                );

                await _taskItemService.UpdateTaskItemAsync(task.Id, updatedItem);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao atualizar status: {ex.Message}", "OK");
            }
        }

        private void OpenManageTaskModal(TaskItem? taskToEdit = null)
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

            IsManageTaskModalOpen = true;
        }

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

            OpenManageTaskModal(SelectedTask);
        }

        private async Task ManageTaskAsync()
        {
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

                    var refreshedFromApi = (await _taskItemService.SearchTaskItemAsync(new TaskItemFilterDTO(UserId: userId)))
                        .FirstOrDefault(t => t.Id == EditingTaskId);

                    if (existingTask != null)
                    {
                        existingTask.Title = NewTaskTitle!;
                        existingTask.Description = NewTaskDescription ?? string.Empty;
                        existingTask.Priority = NewTaskPriority;
                        existingTask.DueDate = DateOnly.FromDateTime(NewTaskDueDate);

                        if (refreshedFromApi != null)
                        {
                            existingTask.Title = refreshedFromApi.Title;
                            existingTask.Description = refreshedFromApi.Description;
                            existingTask.Priority = refreshedFromApi.Priority;
                            existingTask.DueDate = refreshedFromApi.DueDate;
                            existingTask.Status = refreshedFromApi.Status;
                            existingTask.UpdatedAt = refreshedFromApi.UpdatedAt;
                        }

                        if (SelectedTask?.Id == existingTask.Id)
                            SelectedTask = existingTask;

                        await LoadTasksAsync();

                        if (SelectedTask?.Id is int selectedId)
                        {
                            var refreshed = _taskItems.FirstOrDefault(t => t.Id == selectedId);
                            if (refreshed != null)
                                SelectedTask = refreshed;
                        }
                    }
                    else if (SelectedTask?.Id == EditingTaskId)
                    {
                        if (refreshedFromApi != null)
                        {
                            SelectedTask = refreshedFromApi;
                        }
                        else
                        {
                            SelectedTask.Title = NewTaskTitle!;
                            SelectedTask.Description = NewTaskDescription ?? string.Empty;
                            SelectedTask.Priority = NewTaskPriority;
                            SelectedTask.DueDate = DateOnly.FromDateTime(NewTaskDueDate);
                        }
                        OnPropertyChanged(nameof(SelectedTask));
                    }

                    if (SelectedTask?.Id == EditingTaskId && refreshedFromApi != null)
                        SelectedTask = refreshedFromApi;
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

                IsManageTaskModalOpen = false;
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
