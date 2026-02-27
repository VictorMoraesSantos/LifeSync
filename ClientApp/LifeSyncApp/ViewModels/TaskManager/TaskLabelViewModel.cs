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
    public class TaskLabelViewModel : BaseViewModel
    {
        private readonly TaskLabelService _taskLabelService;
        private readonly IUserSession _userSession;
        private bool _isLoadingLabels;

        // Cache management
        private DateTime? _lastLabelsRefresh;

        private ObservableCollection<TaskLabel> _taskLabels = new();
        public ObservableCollection<TaskLabel> TaskLabels
        {
            get => _taskLabels;
            set => SetProperty(ref _taskLabels, value);
        }

        private TaskLabel? _selectedLabel;
        public TaskLabel? SelectedLabel
        {
            get => _selectedLabel;
            set => SetProperty(ref _selectedLabel, value);
        }

        private bool _isManageLabelModalOpen;
        public bool IsManageLabelModalOpen
        {
            get => _isManageLabelModalOpen;
            set => SetProperty(ref _isManageLabelModalOpen, value);
        }

        private string _modalTitle = "Nova Etiqueta";
        public string ModalTitle
        {
            get => _modalTitle;
            set => SetProperty(ref _modalTitle, value);
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        private int? _editingLabelId;
        public int? EditingLabelId
        {
            get => _editingLabelId;
            set => SetProperty(ref _editingLabelId, value);
        }

        private string _labelName = string.Empty;
        public string LabelName
        {
            get => _labelName;
            set
            {
                if (SetProperty(ref _labelName, value))
                    OnPropertyChanged(nameof(CanSaveLabel));
            }
        }

        private LabelColor _selectedLabelColor = LabelColor.Blue;
        public LabelColor SelectedLabelColor
        {
            get => _selectedLabelColor;
            set => SetProperty(ref _selectedLabelColor, value);
        }

        public bool CanSaveLabel => !string.IsNullOrWhiteSpace(LabelName);

        public string SaveButtonText => IsEditMode ? "Atualizar" : "Criar";

        public List<LabelColor> AvailableColors { get; }

        public ICommand GoBackCommand { get; set; }
        public ICommand DeleteLabelCommand { get; set; }
        public ICommand EditLabelCommand { get; set; }
        public ICommand OpenCreateNewLabelCommand { get; set; }
        public ICommand CloseManageLabelModalCommand { get; set; }
        public ICommand SaveLabelCommand { get; set; }
        public ICommand SelectColorCommand { get; set; }

        public TaskLabelViewModel(TaskLabelService taskLabelService, IUserSession userSession)
        {
            _taskLabelService = taskLabelService;
            _userSession = userSession;

            AvailableColors = Enum.GetValues<LabelColor>().ToList();

            GoBackCommand = new Command(async () => await GoBackAsync());
            EditLabelCommand = new Command<TaskLabel>(async (label) => await OpenManageLabelModalAsync(label));
            OpenCreateNewLabelCommand = new Command<TaskLabel>(async (label) => await OpenManageLabelModalAsync(label));
            DeleteLabelCommand = new Command<TaskLabel>(async (label) => await DeleteLabelAsync(label));
            CloseManageLabelModalCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
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

        public async Task LoadLabelsAsync(bool forceRefresh = false)
        {
            if (!forceRefresh && !IsCacheExpired(_lastLabelsRefresh) && TaskLabels.Any())
            {
                System.Diagnostics.Debug.WriteLine("📦 Using cached labels (not expired)");
                return;
            }

            if (_isLoadingLabels)
            {
                System.Diagnostics.Debug.WriteLine("⏳ Labels already loading, skipping duplicate request");
                return;
            }

            try
            {
                _isLoadingLabels = true;
                await MainThread.InvokeOnMainThreadAsync(() => IsBusy = true);

                System.Diagnostics.Debug.WriteLine($"🔄 Loading labels from API (forceRefresh: {forceRefresh})");

                var query = new TaskLabelFilterDTO(UserId: _userSession.UserId, SortBy: "name");
                var labels = await _taskLabelService.SearchTaskLabelAsync(query).ConfigureAwait(false);

                await MainThread.InvokeOnMainThreadAsync(() => TaskLabels.ReplaceAll(labels));

                _lastLabelsRefresh = DateTime.Now;
                System.Diagnostics.Debug.WriteLine($"✅ Labels loaded successfully ({labels.Count()} labels). Cache updated.");
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

        /// <summary>
        /// Force refresh labels from API (used for pull-to-refresh)
        /// </summary>
        public async Task RefreshLabelsAsync()
        {
            System.Diagnostics.Debug.WriteLine("🔃 Force refreshing labels...");
            await LoadLabelsAsync(forceRefresh: true);
        }

        /// <summary>
        /// Invalidate labels cache (call after CREATE/UPDATE/DELETE)
        /// </summary>
        private void InvalidateLabelsCache()
        {
            _lastLabelsRefresh = null;
            System.Diagnostics.Debug.WriteLine("🗑️ Labels cache invalidated");
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

            await Shell.Current.GoToAsync("ManageTaskLabelModal");
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

                await LoadLabelsAsync();

                await Shell.Current.GoToAsync("..");
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

                InvalidateLabelsCache();

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
