using TaskManager.Application.Interfaces;
using TaskManager.Domain.Repositories;

namespace TaskManager.Infrastructure.Services
{
    public class TaskLabelService : ITaskLabelService
    {
        private readonly ITaskLabelRepository _taskLabelRepository;

        public TaskLabelService(ITaskLabelRepository taskLabelRepository)
        {
            _taskLabelRepository = taskLabelRepository;
        }
    }
}
