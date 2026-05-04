using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeSyncApp.Constants;
using LifeSyncApp.Helpers;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Models.TaskManager.Enums;
using LifeSyncApp.Services.TaskManager;
using LifeSyncApp.Services.UserSession;
using System.Collections.ObjectModel;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public partial class TaskLabelViewModel : BaseViewModel
    {
        private readonly ITaskLabelService _taskLabelService;
        private readonly IUserSession _userSession;

        [ObservableProperty]
        private bool _isLoadingLabels;

        // Cache management
        private DateTime? _lastLabelsRefresh;

        [ObservableProperty]
        private ObservableCollection<TaskLabel> _taskLabels = new();

        [ObservableProperty]
        private TaskLabel? _selectedLabel;

        [ObservableProperty]
        private bool _isManageLabelModalOpen;

        [ObservableProperty]
        private string _modalTitle = "Nova Etiqueta";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private bool _isEditMode;

        [ObservableProperty]
        private int? _editingLabelId;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanSaveLabel))]
        private string _labelName = string.Empty;

        [ObservableProperty]
        private LabelColor _selectedLabelColor = LabelColor.Blue;

        public bool CanSaveLabel => !string.IsNullOrWhiteSpace(LabelName);

        public string SaveButtonText => IsEditMode ? "Atualizar" : "Criar";

        public List<LabelColor> AvailableColors { get; }

        public TaskLabelViewModel(ITaskLabelService taskLabelService, IUserSession userSession)
        {
            _taskLabelService = taskLabelService;
            _userSession = userSession;

            AvailableColors = Enum.GetValues<LabelColor>().ToList();
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Erro", $"Erro ao voltar: {ex.Message}", "OK");
                });
            }
        }

        public async Task LoadLabelsAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCacheExpired(_lastLabelsRefresh) && TaskLabels.Any())
                return;

            if (IsLoadingLabels)
                return;

            try
            {
                IsLoadingLabels = true;
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);

                var query = new TaskLabelFilterDTO(UserId: _userSession.UserId, SortBy: "Name");
                var labels = await _taskLabelService.SearchTaskLabelAsync(query).ConfigureAwait(false);

                await MainThread.InvokeOnMainThreadAsync(() => TaskLabels.ReplaceAll(labels));

                _lastLabelsRefresh = DateTime.Now;
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar etiquetas: {ex.Message}", "OK");
                });
            }
            finally
            {
                IsLoadingLabels = false;
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = false);
            }
        }

        public async Task RefreshLabelsAsync()
        {
            await LoadLabelsAsync(forceRefresh: true);
        }

        private void InvalidateLabelsCache()
        {
            _lastLabelsRefresh = null;
        }

        [RelayCommand]
        private async Task EditLabelAsync(TaskLabel? label)
        {
            await OpenManageLabelModalAsync(label);
        }

        [RelayCommand]
        private async Task OpenCreateNewLabelAsync(TaskLabel? label)
        {
            await OpenManageLabelModalAsync(label);
        }

        private async Task OpenManageLabelModalAsync(TaskLabel? label)
        {
            if (label != null)
            {
                IsEditMode = true;
                ModalTitle = "Editar Etiqueta";
                EditingLabelId = label.Id;
                LabelName = label.Name;
                SelectedLabelColor = label.LabelColor;
            }
            else
            {
                IsEditMode = false;
                ModalTitle = "Nova Etiqueta";
                EditingLabelId = null;
                LabelName = string.Empty;
                SelectedLabelColor = LabelColor.Blue;
            }

            await Shell.Current.GoToAsync(AppRoutes.ManageTaskLabelModal);
        }

        [RelayCommand]
        private void SelectColor(LabelColor color)
        {
            SelectedLabelColor = color;
        }

        [RelayCommand]
        private async Task CloseManageLabelModalAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task SaveLabelAsync()
        {
            if (!CanSaveLabel)
                return;

            try
            {
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);

                if (IsEditMode && EditingLabelId.HasValue)
                {
                    var updateDto = new UpdateTaskLabelDTO(
                        Name: LabelName,
                        LabelColor: SelectedLabelColor
                    );

                    await _taskLabelService.EditTaskLabelAsync(EditingLabelId.Value, updateDto).ConfigureAwait(false);
                    InvalidateLabelsCache();

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Sucesso", "Etiqueta atualizada com sucesso!", "OK");
                    });
                }
                else
                {
                    var createDto = new CreateTaskLabelDTO(
                        Name: LabelName,
                        LabelColor: SelectedLabelColor,
                        UserId: _userSession.UserId
                    );

                    await _taskLabelService.CreateTaskLabelAsync(createDto).ConfigureAwait(false);
                    InvalidateLabelsCache();

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Sucesso", "Etiqueta criada com sucesso!", "OK");
                    });
                }

                await LoadLabelsAsync(forceRefresh: true);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.GoToAsync("..");
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Erro", $"Erro ao salvar etiqueta: {ex.Message}", "OK");
                });
            }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = false);
            }
        }

        [RelayCommand]
        private async Task DeleteLabelAsync(TaskLabel label)
        {
            if (label == null) return;

            bool confirm = await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                return await Shell.Current.DisplayAlert(
                    "Excluir Etiqueta",
                    "Tem certeza que deseja excluir esta etiqueta?",
                    "Sim",
                    "Não");
            });

            if (!confirm) return;

            try
            {
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);
                await _taskLabelService.DeleteTaskLabelAsync(label.Id).ConfigureAwait(false);

                InvalidateLabelsCache();
                await LoadLabelsAsync(forceRefresh: true);

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Etiqueta excluída com sucesso!", "OK");
                });
            }
            catch (Exception ex)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Erro", $"Não foi possível excluir a etiqueta: {ex.Message}", "OK");
                });
            }
            finally
            {
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = false);
            }
        }
    }
}
