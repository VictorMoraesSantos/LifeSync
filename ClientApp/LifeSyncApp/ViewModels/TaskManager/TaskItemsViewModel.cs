using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.DTOs.TaskManager.TaskLabel;
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
        private readonly TaskLabelService _taskLabelService;
        private bool _isLoadingTasks;

        // Cache management
        private DateTime? _lastTasksRefresh;
        private DateTime? _lastLabelsRefresh;
        private const int CacheExpirationMinutes = 5;

        private ObservableCollection<TaskItem> _taskItems = new();
        public ObservableCollection<TaskItem> TaskItems
        {
            get => _taskItems;
            set
            {
                _taskItems = value;
                OnPropertyChanged(nameof(TaskItems));
            }
        }

        private ObservableCollection<TaskGroup> _groupedTasks = new();
        public ObservableCollection<TaskGroup> GroupedTasks
        {
            get => _groupedTasks;
            set
            {
                _groupedTasks = value;
                OnPropertyChanged(nameof(GroupedTasks));
            }
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
                if (_editingTaskId != value)
                {
                    _editingTaskId = value;
                    OnPropertyChanged(nameof(EditingTaskId));
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
                if (_newTaskTitle != value)
                {
                    _newTaskTitle = value;
                    OnPropertyChanged(nameof(NewTaskTitle));
                    OnPropertyChanged(nameof(CanManageTask));
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

        private TaskItem? _selectedTask;
        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set
            {
                if (_selectedTask != value)
                {
                    _selectedTask = value;
                    OnPropertyChanged(nameof(SelectedTask));
                }
            }
        }

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

        private ObservableCollection<SelectableLabelItem> _availableLabels = new();
        public ObservableCollection<SelectableLabelItem> AvailableLabels
        {
            get => _availableLabels;
            set
            {
                if (_availableLabels != value)
                {
                    _availableLabels = value;
                    OnPropertyChanged(nameof(AvailableLabels));
                }
            }
        }

        public TaskItemsViewModel(TaskItemService taskItemService, TaskLabelService taskLabelService)
        {
            _taskItemService = taskItemService;
            _taskLabelService = taskLabelService;

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
            FilterViewModel = new FilterTaskItemViewModel(
                onApplyFilters: (status, priority, dateFilter) =>
                {
                    _ = ApplyFiltersAsync(status, priority, dateFilter);
                },
                onCloseModal: async () => await Shell.Current.GoToAsync("..")
            );
        }

        public async Task LoadTasksAsync(bool forceRefresh = false)
        {
            // Check cache validity - only refresh if cache is expired or forced
            if (!forceRefresh && !IsTasksCacheExpired() && TaskItems.Any())
            {
                System.Diagnostics.Debug.WriteLine("📦 Using cached tasks (not expired)");
                return;
            }

            // Avoid concurrent loads
            if (_isLoadingTasks)
            {
                System.Diagnostics.Debug.WriteLine("⏳ Tasks already loading, skipping duplicate request");
                return;
            }

            try
            {
                _isLoadingTasks = true;
                System.Diagnostics.Debug.WriteLine($"🔄 Loading tasks from API (forceRefresh: {forceRefresh})");

                var query = new TaskItemFilterDTO(
                    UserId: 22,
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

                TaskItems = new ObservableCollection<TaskItem>(taskList);
                GroupedTasks = new ObservableCollection<TaskGroup>(grouped);

                // Update cache timestamp
                _lastTasksRefresh = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"✅ Tasks loaded successfully ({taskList.Count} tasks). Cache updated.");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar tarefas: {ex.Message}", "OK");
            }
            finally
            {
                _isLoadingTasks = false;
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
        /// Check if tasks cache has expired
        /// </summary>
        private bool IsTasksCacheExpired()
        {
            if (_lastTasksRefresh == null)
                return true;

            var timeSinceLastRefresh = DateTime.Now - _lastTasksRefresh.Value;
            bool expired = timeSinceLastRefresh.TotalMinutes >= CacheExpirationMinutes;

            if (expired)
                System.Diagnostics.Debug.WriteLine($"⏰ Tasks cache expired (last refresh: {timeSinceLastRefresh.TotalMinutes:F1} minutes ago)");

            return expired;
        }

        /// <summary>
        /// Invalidate tasks cache (call after CREATE/UPDATE/DELETE)
        /// </summary>
        private void InvalidateTasksCache()
        {
            _lastTasksRefresh = null;
            System.Diagnostics.Debug.WriteLine("🗑️ Tasks cache invalidated");
        }

        private async Task ApplyFiltersAsync(Status? status, Priority? priority, DateFilterOption? dateFilter)
        {
            try
            {
                _currentStatusFilter = status;
                _currentPriorityFilter = priority;
                _currentDateFilter = dateFilter ?? DateFilterOption.All;

                await LoadTasksAsync();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao aplicar filtros: {ex.Message}", "OK");
            }
        }

        private async Task NavigateToTaskLabelPage()
        {
            try
            {
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

                // Invalidate cache after update
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

            LoadLabels(selectedIds);

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
                var query = new TaskItemFilterDTO(UserId: 22);
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
                const int userId = 22;

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

                    // Invalidate cache after update
                    InvalidateTasksCache();

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
                        UserId: userId,
                        TaskLabelsId: selectedLabelIds.Count > 0 ? selectedLabelIds : null);
                    var newId = await _taskItemService.CreateTaskItemAsync(dto);

                    // Invalidate cache after create
                    InvalidateTasksCache();

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
                await _taskItemService.DeleteTaskItemAsync(SelectedTask.Id).ConfigureAwait(false);

                // Invalidate cache after delete
                InvalidateTasksCache();

                await Shell.Current.DisplayAlert("Sucesso", "Tarefa excluída com sucesso!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível excluir a tarefa: {ex.Message}", "OK");
            }
        }

        private async void LoadLabels(List<int>? preSelectedIds = null)
        {
            try
            {
                // Check if labels cache is still valid
                if (!IsLabelsCacheExpired() && AvailableLabels.Any())
                {
                    System.Diagnostics.Debug.WriteLine("📦 Using cached labels (not expired)");

                    // Just update selection state
                    foreach (var label in AvailableLabels)
                    {
                        label.IsSelected = preSelectedIds?.Contains(label.Label.Id) ?? false;
                    }
                    return;
                }

                System.Diagnostics.Debug.WriteLine("🔄 Loading labels from API");
                var query = new TaskLabelFilterDTO(UserId: 22, SortBy: "name");
                var labels = await _taskLabelService.SearchTaskLabelAsync(query);
                var selectableLabels = labels.Select(label => new SelectableLabelItem(label, isSelected: preSelectedIds?.Contains(label.Id) ?? false)).ToList();

                AvailableLabels = new ObservableCollection<SelectableLabelItem>(selectableLabels);

                // Update labels cache timestamp
                _lastLabelsRefresh = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"✅ Labels loaded successfully ({labels.Count()} labels). Cache updated.");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar etiquetas: {ex.Message}", "OK");
            }
        }

        /// <summary>
        /// Check if labels cache has expired
        /// </summary>
        private bool IsLabelsCacheExpired()
        {
            if (_lastLabelsRefresh == null)
                return true;

            var timeSinceLastRefresh = DateTime.Now - _lastLabelsRefresh.Value;
            return timeSinceLastRefresh.TotalMinutes >= CacheExpirationMinutes;
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
