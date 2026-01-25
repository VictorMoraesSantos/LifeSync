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

        private readonly ObservableCollection<TaskLabel> _taskLabels = new();
        public ObservableCollection<TaskLabel> TaskLabels => _taskLabels;

        public ICommand GoBackCommand { get; set; }

        public TaskLabelViewModel(TaskLabelService taskLabelService)
        {
            GoBackCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
            _taskLabelService = taskLabelService;
        }

        public async Task LoadLabelsAsync()
        {
            try
            {
                var query = new TaskLabelFilterDTO(UserId: 22, SortBy: "name");
                var labels = await _taskLabelService.SearchTaskLabelAsync(query);

                _taskLabels.Clear();

                foreach (var label in labels)
                    _taskLabels.Add(label);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Erro ao carregar etiquetas: {ex.Message}", "OK");
            }
        }
    }
}
