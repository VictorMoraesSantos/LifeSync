using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.DTOs.TaskManager.TaskLabel;
using LifeSyncApp.Helpers;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Models.TaskManager.Enums;
using LifeSyncApp.Services.TaskManager.Implementation;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public class TaskItemsViewModel : BaseViewModel
    {
        private readonly TaskItemService _taskItemService;
        private readonly TaskLabelService _taskLabelService;
        private readonly IUserSession _userSession;
        private bool _isLoadingTasks;
        public bool IsLoadingTasks
        {
            get => _isLoadingTasks;
            private set => SetProperty(ref _isLoadingTasks, value);
        }

        // Cache management
        private DateTime? _lastTasksRefresh;
        private DateTime? _lastLabelsRefresh;

        private ObservableCollection<TaskItem> _taskItems = new();
        public ObservableCollection<TaskItem> TaskItems
        {
            get => _taskItems;
            set => SetProperty(ref _taskItems, value);
        }

        private ObservableCollection<TaskGroup> _groupedTasks = new();
        public ObservableCollection<TaskGroup> GroupedTasks
        {
            get => _groupedTasks;
            set => SetProperty(ref _groupedTasks, value);
        }

        public FilterTaskItemViewModel FilterViewModel { get; private set; }

        private WeakReference<DatePicker>? _dueDatePickerRef;
        public DatePicker? DueDatePicker
        {
            get
            {
                if (_dueDatePickerRef != null && _dueDatePickerRef.TryGetTarget(out var picker))
                    return picker;
                return null;
            }
            set
            {
                _dueDatePickerRef = value != null ? new WeakReference<DatePicker>(value) : null;
            }
        }

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
        public ICommand ToggleLabelCommand { get; }

        private bool _isManageTaskModalOpen;
        public bool IsManageTaskModalOpen
        {
            get => _isManageTaskModalOpen;
            set => SetProperty(ref _isManageTaskModalOpen, value);
        }

        private bool _isFilterTaskModalOpen;
        public bool IsFilterTaskModalOpen
        {
            get => _isFilterTaskModalOpen;
            set => SetProperty(ref _isFilterTaskModalOpen, value);
        }

        private int? _editingTaskId;
        public int? EditingTaskId
        {
            get => _editingTaskId;
            set
            {
                if (SetProperty(ref _editingTaskId, value))
                {
                    OnPropertyChanged(nameof(IsEditMode));
                    OnPropertyChanged(nameof(ModalTitle));
                    OnPropertyChanged(nameof(SaveButtonText));
                }
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
                if (SetProperty(ref _newTaskTitle, value))
                    OnPropertyChanged(nameof(CanManageTask));
            }
        }

        private string? _newTaskDescription;
        public string? NewTaskDescription
        {
            get => _newTaskDescription;
            set => SetProperty(ref _newTaskDescription, value);
        }

        private TaskItem? _selectedTask;
        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set => SetProperty(ref _selectedTask, value);
        }

        private Priority _newTaskPriority = Priority.Medium;
        public Priority NewTaskPriority
        {
            get => _newTaskPriority;
            set => SetProperty(ref _newTaskPriority, value);
        }

        private DateTime _newTaskDueDate = DateTime.Today;
        public DateTime NewTaskDueDate
        {
            get => _newTaskDueDate;
            set => SetProperty(ref _newTaskDueDate, value);
        }

        private ObservableCollection<SelectableLabelItem> _availableLabels = new();
        public ObservableCollection<SelectableLabelItem> AvailableLabels
        {
            get => _availableLabels;
            set => SetProperty(ref _availableLabels, value);
        }

        public TaskItemsViewModel(TaskItemService taskItemService, TaskLabelService taskLabelService, IUserSession userSession)
        {
            _taskItemService = taskItemService;
            _taskLabelService = taskLabelService;
            _userSession = userSession;

            GoToLabelsCommand = new Command(async () => await NavigateToTaskLabelPage());
            ToggleStatusCommand = new Command<TaskItem>(async (task) => await ToggleStatusAsync(task));
            OpenManageTaskModalCommand = new Command<TaskItem>(async (task) => await OpenManageTaskModalAsync(task));
            CloseManageTaskModalCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            ManageTaskCommand = new Command(async () => await ManageTaskAsync());
            SetPriorityCommand = new Command<Priority>(p => NewTaskPriority = p);
            FocusDueDatePickerCommand = new Command(() => { });
            ViewTaskDetailCommand = new Command<TaskItem>(async (task) => await NavigateToTaskDetailAsync(task));
            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            EditTaskCommand = new Command(async () => await EditTaskAsync());
            DeleteTaskCommand = new Command(async () => await DeleteTaskAsync());
            OpenFiltersCommand = new Command(async () => await OpenFiltersModalAsync());
            ToggleLabelCommand = new Command<SelectableLabelItem>(ToggleLabel);

            FilterViewModel = new FilterTaskItemViewModel();
            FilterViewModel.FiltersApplied += (s, e) =>
            {
                _currentStatusFilter = e.Status;
                _currentPriorityFilter = e.Priority;
                _currentDateFilter = e.DateFilter ?? DateFilterOption.All;
                InvalidateTasksCache();
            };
            FilterViewModel.Closed += async (s, e) => await Shell.Current.GoToAsync("..");
        }

        public async Task LoadTasksAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCacheExpired(_lastTasksRefresh) && TaskItems.Any())
            {
                System.Diagnostics.Debug.WriteLine("📦 Using cached tasks (not expired)");
                return;
            }

            if (IsLoadingTasks)
            {
                System.Diagnostics.Debug.WriteLine("⏳ Tasks already loading, skipping duplicate request");
                return;
            }

            try
            {
                IsLoadingTasks = true;
                System.Diagnostics.Debug.WriteLine($"🔄 Loading tasks from API (forceRefresh: {forceRefresh})");

                var query = new TaskItemFilterDTO(
                    UserId: _userSession.UserId,
                    Status: _currentStatusFilter,
                    Priority: _currentPriorityFilter,
                    DueDate: GetDueDateFromFilter(_currentDateFilter));

                var tasks = await _taskItemService.SearchTaskItemAsync(query).ConfigureAwait(false);
                var taskList = tasks.ToList();
                var grouped = taskList
                    .OrderBy(t => t.DueDate)
                    .GroupBy(t => t.DueDate)
                    .Select(g => new TaskGroup(g.Key, g))
                    .ToList();

                // Batch UI updates on main thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _taskItems.ReplaceAll(taskList);
                    _groupedTasks.ReplaceAll(grouped);
                });

                _lastTasksRefresh = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"✅ Tasks loaded successfully ({taskList.Count} tasks). Cache updated.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading tasks: {ex.Message}");
            }
            finally
            {
                IsLoadingTasks = false;
            }
        }

        /// <summary>
        /// Force refresh tasks from API (used for pull-to-refresh)
        /// </summary>
        public async Task RefreshTasksAsync()
        {
            System.Diagnostics.Debug.WriteLine("🔃 Force refreshing tasks...");
            await LoadTasksAsync(forceRefresh: true);
        }

        /// <summary>
        /// Invalidate tasks cache (call after CREATE/UPDATE/DELETE)
        /// </summary>
        private void InvalidateTasksCache()
        {
            _lastTasksRefresh = null;
            System.Diagnostics.Debug.WriteLine("🗑️ Tasks cache invalidated");
        }

        private void ApplyFilters(Status? status, Priority? priority, DateFilterOption? dateFilter)
        {
            _currentStatusFilter = status;
            _currentPriorityFilter = priority;
            _currentDateFilter = dateFilter ?? DateFilterOption.All;
            InvalidateTasksCache();
        }

        private async Task NavigateToTaskLabelPage()
        {
            try
            {
                _lastLabelsRefresh = null;
                await Shell.Current.GoToAsync("tasklabels");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar: {ex.Message}", "OK");
            }
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
                    task.DueDate,
                    task.Labels?.Select(l => l.Id).ToList()
                );

                await _taskItemService.UpdateTaskItemAsync(task.Id, updatedItem);

                InvalidateTasksCache();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao atualizar status: {ex.Message}", "OK");
            }
        }

        private async Task OpenManageTaskModalAsync(TaskItem? taskToEdit = null)
        {
            List<int> selectedIds = new();

            if (taskToEdit != null)
            {
                EditingTaskId = taskToEdit.Id;
                NewTaskTitle = taskToEdit.Title;
                NewTaskDescription = taskToEdit.Description;
                NewTaskPriority = taskToEdit.Priority;
                NewTaskDueDate = taskToEdit.DueDate.ToDateTime(TimeOnly.MinValue);

                selectedIds = taskToEdit.Labels.Select(l => l.Id).ToList();
            }
            else
            {
                EditingTaskId = null;
                NewTaskTitle = null;
                NewTaskDescription = null;
                NewTaskPriority = Priority.Medium;
                NewTaskDueDate = DateTime.Today;
            }

            await LoadLabelsAsync(selectedIds);

            await Shell.Current.GoToAsync("ManageTaskItemModal");
        }

        private async Task OpenFiltersModalAsync()
        {
            await Shell.Current.GoToAsync("FilterTaskItemPopup");
        }

        private async Task NavigateToTaskDetailAsync(TaskItem task)
        {
            if (task == null) return;

            try
            {
                SelectedTask = task;
                await Shell.Current.GoToAsync($"taskdetail?taskId={task.Id}");
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Erro", $"Erro ao abrir detalhes: {ex.Message}", "OK");
                });
            }
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
                var query = new TaskItemFilterDTO(UserId: _userSession.UserId);
                var tasks = await _taskItemService.SearchTaskItemAsync(query);
                var foundTask = tasks.FirstOrDefault(t => t.Id == taskId);
                SelectedTask = foundTask;
            }
        }

        private async Task EditTaskAsync()
        {
            if (SelectedTask == null) return;
            await OpenManageTaskModalAsync(SelectedTask);
        }

        private async Task ManageTaskAsync()
        {
            try
            {
                if (IsEditMode)
                {
                    var existingTask = _taskItems.FirstOrDefault(t => t.Id == EditingTaskId);
                    var currentStatus = existingTask?.Status ?? Status.Pending;

                    var selectedLabels = AvailableLabels
                        .Where(l => l.IsSelected)
                        .Select(l => l.Label)
                        .ToList();

                    var selectedLabelIds = selectedLabels.Select(l => l.Id).ToList();

                    var updateDto = new UpdateTaskItemDTO(
                        Title: NewTaskTitle!,
                        Description: NewTaskDescription ?? string.Empty,
                        Status: currentStatus,
                        Priority: NewTaskPriority,
                        DueDate: DateOnly.FromDateTime(NewTaskDueDate),
                        TaskLabelsId: selectedLabelIds.Count > 0 ? selectedLabelIds : null
                    );

                    await _taskItemService.UpdateTaskItemAsync(EditingTaskId!.Value, updateDto);

                    _lastTasksRefresh = DateTime.Now;

                    if (existingTask != null)
                    {
                        existingTask.Title = NewTaskTitle!;
                        existingTask.Description = NewTaskDescription ?? string.Empty;
                        existingTask.Priority = NewTaskPriority;
                        existingTask.DueDate = DateOnly.FromDateTime(NewTaskDueDate);
                        existingTask.UpdatedAt = DateTime.UtcNow;
                        existingTask.Labels = selectedLabels;

                        if (SelectedTask?.Id == existingTask.Id)
                            SelectedTask = existingTask;

                        OnPropertyChanged(nameof(SelectedTask));
                    }
                    else if (SelectedTask?.Id == EditingTaskId)
                    {
                        SelectedTask.Title = NewTaskTitle!;
                        SelectedTask.Description = NewTaskDescription ?? string.Empty;
                        SelectedTask.Priority = NewTaskPriority;
                        SelectedTask.DueDate = DateOnly.FromDateTime(NewTaskDueDate);
                        SelectedTask.UpdatedAt = DateTime.UtcNow;
                        SelectedTask.Labels = selectedLabels;
                        OnPropertyChanged(nameof(SelectedTask));
                    }
                }
                else
                {
                    var selectedLabels = AvailableLabels
                        .Where(l => l.IsSelected)
                        .Select(l => l.Label)
                        .ToList();

                    var selectedLabelIds = selectedLabels.Select(l => l.Id).ToList();

                    var dto = new CreateTaskItemDTO(
                        Title: NewTaskTitle!,
                        Description: NewTaskDescription ?? string.Empty,
                        Priority: NewTaskPriority,
                        DueDate: DateOnly.FromDateTime(NewTaskDueDate),
                        UserId: _userSession.UserId,
                        TaskLabelsId: selectedLabelIds.Count > 0 ? selectedLabelIds : null);
                    var newId = await _taskItemService.CreateTaskItemAsync(dto);

                    _lastTasksRefresh = DateTime.Now;

                    var created = new TaskItem
                    {
                        Id = newId,
                        Title = dto.Title,
                        Description = dto.Description,
                        Priority = dto.Priority,
                        DueDate = dto.DueDate,
                        Status = Status.Pending,
                        UserId = dto.UserId,
                        CreatedAt = DateTime.UtcNow,
                        Labels = selectedLabels
                    };

                    InsertTaskIntoGroups(created);
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível salvar a tarefa: {ex.Message}", "OK");
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
                await _taskItemService.DeleteTaskItemAsync(SelectedTask.Id);

                InvalidateTasksCache();

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Tarefa excluída com sucesso!", "OK");
                    await Shell.Current.GoToAsync("..");
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Erro", $"Não foi possível excluir a tarefa: {ex.Message}", "OK");
                });
            }
        }

        private async Task LoadLabelsAsync(List<int>? preSelectedIds = null)
        {
            try
            {
                if (!IsCacheExpired(_lastLabelsRefresh) && AvailableLabels.Any())
                {
                    System.Diagnostics.Debug.WriteLine("📦 Using cached labels (not expired)");

                    foreach (var label in AvailableLabels)
                        label.IsSelected = preSelectedIds?.Contains(label.Label.Id) ?? false;

                    return;
                }

                System.Diagnostics.Debug.WriteLine("🔄 Loading labels from API");
                var query = new TaskLabelFilterDTO(UserId: _userSession.UserId, SortBy: "Name");
                var labels = await _taskLabelService.SearchTaskLabelAsync(query);
                var selectableLabels = labels.Select(label => new SelectableLabelItem(label, isSelected: preSelectedIds?.Contains(label.Id) ?? false)).ToList();

                await MainThread.InvokeOnMainThreadAsync(() => _availableLabels.ReplaceAll(selectableLabels));

                _lastLabelsRefresh = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"✅ Labels loaded successfully ({labels.Count()} labels). Cache updated.");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar etiquetas: {ex.Message}", "OK");
            }
        }

        private void ToggleLabel(SelectableLabelItem labelItem)
        {
            if (labelItem == null) return;
            labelItem.IsSelected = !labelItem.IsSelected;
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
