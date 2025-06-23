using Core.Domain.Entities;
using Core.Domain.Exceptions;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Entities
{
    public class TaskItem : BaseEntity<int>
    {
        public int UserId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Status Status { get; private set; }
        public Priority Priority { get; private set; }
        public DateOnly DueDate { get; private set; }

        private readonly List<TaskLabel> _labels = new();
        public IReadOnlyCollection<TaskLabel> Labels => _labels.AsReadOnly();

        protected TaskItem() { }

        public TaskItem(string title, string description, Priority priority, DateOnly dueDate, int userId)
        {
            SetTitle(title);
            SetDescription(description);
            Status = Status.Pending;
            Priority = priority;
            SetDueDate(dueDate);
            UserId = userId;
        }

        public void Update(string title, string description, Status status, Priority priority, DateOnly dueDate)
        {
            SetTitle(title);
            SetDescription(description);
            Status = status;
            Priority = priority;
            MarkAsUpdated();
        }

        private void SetTitle(string title)
        {
            ValidateString(title);
            Title = title.Trim();
        }

        private void SetDescription(string description)
        {
            ValidateString(description);
            Description = description.Trim();
        }

        private void SetDueDate(DateOnly dueDate)
        {
            if (dueDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new DomainException("Due date cannot be in the past.");
            DueDate = dueDate;
        }

        public void ChangeStatus(Status status)
        {
            Status = status;
            MarkAsUpdated();
        }

        public void AddLabel(TaskLabel label)
        {
            if (label == null)
                throw new DomainException("Label cannot be null.");
            if (_labels.Any(l => l == label))
                throw new DomainException("Label already exists for this task.");
            _labels.Add(label);
            MarkAsUpdated();
        }

        public void RemoveLabel(TaskLabel label)
        {
            if (label == null)
                throw new DomainException("Label cannot be null.");
            if (!_labels.Remove(label))
                throw new DomainException("Label not found in this task.");
            MarkAsUpdated();
        }

        private void ValidateString(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Value cannot be null or empty.");
        }
    }
}