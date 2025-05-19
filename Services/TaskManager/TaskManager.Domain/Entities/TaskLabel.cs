using Core.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskLabel : BaseEntity<int>
    {
        public string Name { get; private set; }
        public LabelColor LabelColor { get; private set; }
        public int UserId { get; private set; }
        public int TaskItemId { get; private set; }
        public TaskItem TaskItem { get; private set; }

        protected TaskLabel() { }

        public TaskLabel(string name, LabelColor labelColor, int userId, int taskItemId)
        {
            Validate(name);
            Name = name.Trim();
            LabelColor = labelColor;
            UserId = userId;
            TaskItemId = taskItemId;
        }

        public void Update(string name, LabelColor labelColor)
        {
            Validate(name);
            Name = name;
            LabelColor = labelColor;
            MarkAsUpdated();
        }

        private void Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
        }
    }
}