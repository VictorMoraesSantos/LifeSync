using LifeSyncApp.DTOs.TaskManager.TaskLabel;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Models.TaskManager.Enums;
using LifeSyncApp.Services.TaskManager.Implementation;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public class TaskLabelViewModel : BaseViewModel
    {
        private readonly TaskLabelService _taskLabelService;
        private bool _isLoadingLabels;

        private ObservableCollection<TaskLabel> _taskLabels = new();
        public ObservableCollection<TaskLabel> TaskLabels
        {
            get => _taskLabels;
            set
            {
                if (_taskLabels != value)
                {
                    _taskLabels = value;
                    OnPropertyChanged(nameof(TaskLabels));
                }
            }
        }

        private TaskLabel? _selectedLabel;
        public TaskLabel? SelectedLabel
        {
            get => _selectedLabel;
            set
            {
                if (_selectedLabel != value)
                {
                    _selectedLabel = value;
                    OnPropertyChanged(nameof(SelectedLabel));
                }
            }
        }

        // Modal state properties
        private bool _isManageLabelModalOpen;
        public bool IsManageLabelModalOpen
        {
            get => _isManageLabelModalOpen;
            set
            {
                if (_isManageLabelModalOpen != value)
                {
                    _isManageLabelModalOpen = value;
                    OnPropertyChanged(nameof(IsManageLabelModalOpen));
                }
            }
        }

        private string _modalTitle = "Nova Etiqueta";
        public string ModalTitle
        {
            get => _modalTitle;
            set
            {
                if (_modalTitle != value)
                {
                    _modalTitle = value;
                    OnPropertyChanged(nameof(ModalTitle));
                }
            }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (_isEditMode != value)
                {
                    _isEditMode = value;
                    OnPropertyChanged(nameof(IsEditMode));
                }
            }
        }

        private int? _editingLabelId;
        public int? EditingLabelId
        {
            get => _editingLabelId;
            set
            {
                if (_editingLabelId != value)
                {
                    _editingLabelId = value;
                    OnPropertyChanged(nameof(EditingLabelId));
                }
            }
        }

        // Form properties
        private string _labelName = string.Empty;
        public string LabelName
        {
            get => _labelName;
            set
            {
                if (_labelName != value)
                {
                    _labelName = value;
                    OnPropertyChanged(nameof(LabelName));
                    OnPropertyChanged(nameof(CanSaveLabel));
                }
            }
        }

        private LabelColor _selectedLabelColor = LabelColor.Blue;
        public LabelColor SelectedLabelColor
        {
            get => _selectedLabelColor;
            set
            {
                if (_selectedLabelColor != value)
                {
                    _selectedLabelColor = value;
                    OnPropertyChanged(nameof(SelectedLabelColor));
                }
            }
        }

        public bool CanSaveLabel => !string.IsNullOrWhiteSpace(LabelName);

        public string SaveButtonText => IsEditMode ? "Atualizar" : "Criar";

        // Available colors for selection
        public List<LabelColor> AvailableColors { get; }

        public ICommand GoBackCommand { get; set; }
        public ICommand DeleteLabelCommand { get; set; }
        public ICommand EditLabelCommand { get; set; }
        public ICommand OpenCreateNewLabelCommand { get; set; }
        public ICommand CloseManageLabelModalCommand { get; set; }
        public ICommand SaveLabelCommand { get; set; }
        public ICommand SelectColorCommand { get; set; }

        public TaskLabelViewModel(TaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;

            // Initialize available colors list
            AvailableColors = Enum.GetValues<LabelColor>().ToList();

            GoBackCommand = new Command(async () => await GoBackAsync());
            EditLabelCommand = new Command<TaskLabel>(OpenManageLabelModal);
            OpenCreateNewLabelCommand = new Command<TaskLabel>(OpenManageLabelModal);
            DeleteLabelCommand = new Command<TaskLabel>(async (label) => await DeleteLabelAsync(label));
            CloseManageLabelModalCommand = new Command(CloseManageLabelModal);
            SaveLabelCommand = new Command(async () => await SaveLabelAsync(), () => CanSaveLabel);
            SelectColorCommand = new Command<LabelColor>(SelectColor);
        }

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

        public async Task LoadLabelsAsync()
        {
            // Evitar múltiplas chamadas simultâneas
            if (_isLoadingLabels)
                return;

            try
            {
                _isLoadingLabels = true;
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);

                var query = new TaskLabelFilterDTO(UserId: 22, SortBy: "name");
                var labels = await _taskLabelService.SearchTaskLabelAsync(query).ConfigureAwait(false);

                // Processar em background thread
                var labelCollection = await Task.Run(() =>
                    new ObservableCollection<TaskLabel>(labels)
                ).ConfigureAwait(false);

                // Atualizar UI na main thread
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    TaskLabels = labelCollection;
                });
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
                _isLoadingLabels = false;
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = false);
            }
        }

        private void OpenManageLabelModal(TaskLabel? label)
        {
            if (label != null)
            {
                // Edit mode
                IsEditMode = true;
                ModalTitle = "Editar Etiqueta";
                EditingLabelId = label.Id;
                LabelName = label.Name;
                SelectedLabelColor = label.LabelColor;
            }
            else
            {
                // Create mode
                IsEditMode = false;
                ModalTitle = "Nova Etiqueta";
                EditingLabelId = null;
                LabelName = string.Empty;
                SelectedLabelColor = LabelColor.Blue;
            }

            IsManageLabelModalOpen = true;
        }

        private void CloseManageLabelModal()
        {
            IsManageLabelModalOpen = false;

            // Reset form
            LabelName = string.Empty;
            SelectedLabelColor = LabelColor.Blue;
            EditingLabelId = null;
            IsEditMode = false;
        }

        private void SelectColor(LabelColor color)
        {
            SelectedLabelColor = color;
        }

        private async Task SaveLabelAsync()
        {
            if (!CanSaveLabel)
                return;

            try
            {
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);

                if (IsEditMode && EditingLabelId.HasValue)
                {
                    // Update existing label
                    var updateDto = new UpdateTaskLabelDTO(
                        Name: LabelName,
                        LabelColor: SelectedLabelColor
                    );

                    await _taskLabelService.EditTaskLabelAsync(EditingLabelId.Value, updateDto).ConfigureAwait(false);

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Sucesso", "Etiqueta atualizada com sucesso!", "OK");
                    });
                }
                else
                {
                    // Create new label
                    var createDto = new CreateTaskLabelDTO(
                        Name: LabelName,
                        LabelColor: SelectedLabelColor,
                        UserId: 22 // TODO: Get from auth service
                    );

                    await _taskLabelService.CreateTaskLabelAsync(createDto).ConfigureAwait(false);

                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Sucesso", "Etiqueta criada com sucesso!", "OK");
                    });
                }

                // Reload labels list
                await LoadLabelsAsync();

                // Close modal
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    CloseManageLabelModal();
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

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Sucesso", "Etiqueta excluída com sucesso!", "OK");
                    await Shell.Current.GoToAsync("..");
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
