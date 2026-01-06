using Core.Domain.Entities;
using Core.Domain.Exceptions;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Errors;

namespace TaskManager.Domain.Entities
{
    public class TaskLabel : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public LabelColor LabelColor { get; private set; }

        private readonly List<TaskItem> _items = new();
        public IReadOnlyCollection<TaskItem> Items => _items.AsReadOnly();

        protected TaskLabel() { }

        public TaskLabel(string name, LabelColor labelColor, int userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(TaskLabelErrors.InvalidName);

            Name = name.Trim();
            LabelColor = labelColor;
            UserId = userId;
        }

        public void Update(string name, LabelColor labelColor)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(TaskLabelErrors.InvalidName);

            Name = name.Trim();
            LabelColor = labelColor;
            MarkAsUpdated();
        }

        public void AddTaskItem(TaskItem item)
        {
            if (item == null)
                throw new DomainException(TaskLabelErrors.NullItem);

            if (_items.Any(i => i.Id == item.Id))
                throw new DomainException(TaskLabelErrors.DuplicateItem);

            _items.Add(item);
            MarkAsUpdated();
        }

        public void RemoveTaskItem(TaskItem item)
        {
            if (item == null)
                throw new DomainException(TaskLabelErrors.NullItem);

            var existingItem = _items.FirstOrDefault(i => i.Id == item.Id);
            if (existingItem == null)
                throw new DomainException(TaskLabelErrors.ItemNotFound);

            _items.Remove(existingItem);
            MarkAsUpdated();
        }
    }
}