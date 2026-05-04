using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.DTOs.TaskManager.TaskLabel;
using LifeSyncApp.Helpers;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Models.TaskManager.Enums;
using LifeSyncApp.Services.TaskManager;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public partial class TaskItemsViewModel : BaseViewModel
    {
        private readonly ITaskItemService _taskItemService;
        private readonly ITaskLabelService _taskLabelService;
        private readonly IUserSession _userSession;

        [ObservableProperty]
        private bool _isLoadingTasks;

        // Cache management
        private DateTime? _lastTasksRefresh;
        private DateTime? _lastLabelsRefresh;

        [ObservableProperty]
        private ObservableCollection<TaskItem> _taskItems = new();

        [ObservableProperty]
        private ObservableCollection<TaskGroup> _groupedTasks = new();

        public FilterTaskItemViewModel FilterViewModel { get; private set; }

        private Status? _currentStatusFilter;
        private Priority? _currentPriorityFilter;
        private DateFilterOption? _currentDateFilter = DateFilterOption.All;

        [ObservableProperty]
        private bool _isManageTaskModalOpen;

        [ObservableProperty]
        private bool _isFilterTaskModalOpen;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditMode))]
        [NotifyPropertyChangedFor(nameof(ModalTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private int? _editingTaskId;

        public bool CanManageTask => !string.IsNullOrWhiteSpace(NewTaskTitle);
        public bool IsEditMode => EditingTaskId.HasValue;
        public string ModalTitle => IsEditMode ? "Editar Tarefa" : "Nova Tarefa";
        public string SaveButtonText => IsEditMode ? "Salvar Alterações" : "Criar Tarefa";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanManageTask))]
        private string? _newTaskTitle;

        [ObservableProperty]
        private string? _newTaskDescription;

        [ObservableProperty]
        private TaskItem? _selectedTask;

        [ObservableProperty]
        private Priority _newTaskPriority = Priority.Medium;

        [ObservableProperty]
        private DateTime _newTaskDueDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<SelectableLabelItem> _availableLabels = new();

        public TaskItemsViewModel(ITaskItemService taskItemService, ITaskLabelService taskLabelService, IUserSession userSession)
        {
            _taskItemService = taskItemService;
            _taskLabelService = taskLabelService;
            _userSession = userSession;

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
                return;

            if (IsLoadingTasks)
                return;

            try
            {
                IsLoadingTasks = true;

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

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TaskItems.ReplaceAll(taskList);
                    GroupedTasks.ReplaceAll(grouped);
                });

                _lastTasksRefresh = DateTime.Now;
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

        public async Task RefreshTasksAsync()
        {
            await LoadTasksAsync(forceRefresh: true);
        }

        private void InvalidateTasksCache()
        {
            _lastTasksRefresh = null;
        }

        [RelayCommand]
        private async Task ToggleStatusAsync(TaskItem task)
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

        [RelayCommand]
        private async Task GoToLabelsAsync()
        {
            try
            {
                _lastLabelsRefresh = null;
                await Shell.Current.GoToAsync(AppRoutes.TaskLabels);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
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
            await Shell.Current.GoToAsync(AppRoutes.ManageTaskItemModal);
        }

        [RelayCommand]
        private async Task CloseManageTaskModalAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task ManageTaskAsync()
        {
            try
            {
                if (IsEditMode)
                {
                    var existingTask = TaskItems.FirstOrDefault(t => t.Id == EditingTaskId);
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

        [RelayCommand]
        private void SetPriority(Priority p) => NewTaskPriority = p;

        [RelayCommand]
        private async Task ViewTaskDetailAsync(TaskItem task)
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

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task EditTaskAsync()
        {
            if (SelectedTask == null) return;
            await OpenManageTaskModalAsync(SelectedTask);
        }

        [RelayCommand]
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

        [RelayCommand]
        private async Task OpenFiltersAsync()
        {
            await Shell.Current.GoToAsync(AppRoutes.FilterTaskItemPopup);
        }

        [RelayCommand]
        private void ToggleLabel(SelectableLabelItem labelItem)
        {
            if (labelItem == null) return;
            labelItem.IsSelected = !labelItem.IsSelected;
        }

        public async Task LoadTaskDetailAsync(int taskId)
        {
            var task = TaskItems.FirstOrDefault(t => t.Id == taskId);
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

        private async Task LoadLabelsAsync(List<int>? preSelectedIds = null)
        {
            try
            {
                if (!IsCacheExpired(_lastLabelsRefresh) && AvailableLabels.Any())
                {
                    foreach (var label in AvailableLabels)
                        label.IsSelected = preSelectedIds?.Contains(label.Label.Id) ?? false;
                    return;
                }

                var query = new TaskLabelFilterDTO(UserId: _userSession.UserId, SortBy: "Name");
                var labels = await _taskLabelService.SearchTaskLabelAsync(query);
                var selectableLabels = labels.Select(label => new SelectableLabelItem(label, isSelected: preSelectedIds?.Contains(label.Id) ?? false)).ToList();

                await MainThread.InvokeOnMainThreadAsync(() => AvailableLabels.ReplaceAll(selectableLabels));

                _lastLabelsRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar etiquetas: {ex.Message}", "OK");
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

        private void InsertTaskIntoGroups(TaskItem task)
        {
            TaskItems.Add(task);

            var existingGroup = GroupedTasks.FirstOrDefault(g => g.DueDate == task.DueDate);
            if (existingGroup is null)
            {
                var newGroup = new TaskGroup(task.DueDate, new[] { task });

                var insertIndex = 0;
                while (insertIndex < GroupedTasks.Count && GroupedTasks[insertIndex].DueDate < task.DueDate)
                    insertIndex++;

                GroupedTasks.Insert(insertIndex, newGroup);
                return;
            }

            existingGroup.Add(task);

            var index = GroupedTasks.IndexOf(existingGroup);
            GroupedTasks[index] = new TaskGroup(existingGroup.DueDate, existingGroup);
        }
    }
}
