using LifeSyncApp.DTOs.TaskManager.TaskItem;
using LifeSyncApp.Models.TaskManager;
using LifeSyncApp.Services.TaskManager.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LifeSyncApp.ViewModels.TaskManager
{
    public class TaskItemsViewModel : BaseViewModel
    {
        private readonly TaskItemService _taskItemService;
        private readonly ObservableCollection<TaskItem> _taskItems = new();

        public ObservableCollection<TaskItem> TaskItems => _taskItems;
        public ICommand LoadTasksCommand { get; }

        public TaskItemsViewModel(TaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
            LoadTasksCommand = new Command(async () => await LoadTasksAsync());
        }

        public async Task LoadTasksAsync()
        {
            try
            {
                var query = new TaskItemFilterDTO(UserId: 22);
                var tasks = await _taskItemService.SearchTaskItemAsync(query);
                _taskItems.Clear();
                foreach (var task in tasks)
                {
                    _taskItems.Add(task);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Erro",
                    $"Erro ao carregar tarefas: {ex.Message}",
                    "OK"
                );
            }
        }
    }
}
