using Core.Domain.Entities;
using Core.Domain.Exceptions;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Errors;

namespace TaskManager.Domain.Entities
{
    public class TaskLabel : BaseEntity<int>
    {
        public string Name { get; private set; }
        public LabelColor LabelColor { get; private set; }
        public int UserId { get; private set; }
        public int? TaskItemId { get; private set; }
        public TaskItem? TaskItem { get; private set; }

        protected TaskLabel() { }

        public TaskLabel(string name, LabelColor labelColor, int userId, int? taskItemId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(TaskLabelErrors.InvalidName);

            Name = name.Trim();
            LabelColor = labelColor;
            UserId = userId;
            TaskItemId = taskItemId;
        }

        public void Update(string name, LabelColor labelColor)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(TaskLabelErrors.InvalidName);

            Name = name.Trim();
            LabelColor = labelColor;

            MarkAsUpdated();
        }
    }
}