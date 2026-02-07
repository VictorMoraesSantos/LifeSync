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

        private ObservableCollection<TaskLabel> _taskLabels = new();
        public ObservableCollection<TaskLabel> TaskLabels
        {
            get => _taskLabels;
            set
            {
                _taskLabels = value;
                OnPropertyChanged(nameof(TaskLabels));
            }
        }

        private TaskLabel? _selectedLabel;
        public TaskLabel? SelectedLabel
        {
            get => _selectedLabel; set
            {
                _selectedLabel = value;
                OnPropertyChanged(nameof(SelectedLabel));
            }
        }

        public ICommand GoBackCommand { get; set; }
        public ICommand DeleteLabelCommand { get; set; }
        public ICommand OpenCreateNewLabelCommand { get; set; }

        public TaskLabelViewModel(TaskLabelService taskLabelService)
        {
            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            _taskLabelService = taskLabelService;

            OpenCreateNewLabelCommand = new Command<TaskLabel>(OpenManageLabelModal);
            DeleteLabelCommand = new Command<TaskLabel>(async (label) => await DeleteLabelAsync(label));
        }

        public async Task LoadLabelsAsync()
        {
            try
            {
                var query = new TaskLabelFilterDTO(UserId: 22, SortBy: "name");
                var labels = await _taskLabelService.SearchTaskLabelAsync(query);

                TaskLabels = new ObservableCollection<TaskLabel>(labels);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Erro ao carregar etiquetas: {ex.Message}", "OK");
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
            if (SelectedLabel == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Excluir Etiqueta",
                "Tem certeza que deseja excluir esta etiqueta?",
                "Sim",
                "Não");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                await _taskLabelService.DeleteTaskLabelAsync(label.Id);

                await Shell.Current.DisplayAlert("Sucesso", "Tarefa excluída com sucesso!", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Não foi possível excluir a etiqueta: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
