using LifeSyncApp.DTOs.TaskManager.TaskLabel;
using LifeSyncApp.Models.TaskManager;
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

        public ICommand GoBackCommand { get; set; }
        public ICommand DeleteLabelCommand { get; set; }
        public ICommand EditLabelCommand { get; set; }
        public ICommand OpenCreateNewLabelCommand { get; set; }

        public TaskLabelViewModel(TaskLabelService taskLabelService)
        {
            _taskLabelService = taskLabelService;

            GoBackCommand = new Command(async () => await GoBackAsync());
            EditLabelCommand = new Command<TaskLabel>(OpenManageLabelModal);
            OpenCreateNewLabelCommand = new Command<TaskLabel>(OpenManageLabelModal);
            DeleteLabelCommand = new Command<TaskLabel>(async (label) => await DeleteLabelAsync(label));
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

            }
            else
            {

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
