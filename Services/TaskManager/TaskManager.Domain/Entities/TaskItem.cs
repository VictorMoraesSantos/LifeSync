using Core.Domain.Entities;
using Core.Domain.Exceptions;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Errors;

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

        public TaskItem(
            string title,
            string description,
            Priority priority,
            DateOnly dueDate,
            int userId)
        {
            SetTitle(title);
            SetDescription(description);
            Status = Status.Pending;
            Priority = priority;
            SetDueDate(dueDate);
            UserId = userId;
        }

        public void Update(
            string title,
            string description,
            Status status,
            Priority priority,
            DateOnly dueDate)
        {
            SetTitle(title);
            SetDescription(description);
            Status = status;
            Priority = priority;
            SetDueDate(dueDate);
            MarkAsUpdated();
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException(TaskItemErrors.InvalidTitle);

            Title = title.Trim();
        }

        private void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException(TaskItemErrors.InvalidDescription);

            Description = description.Trim();
        }

        private void SetDueDate(DateOnly dueDate)
        {
            if (dueDate < DateOnly.FromDateTime(DateTime.Today))
                throw new DomainException(TaskItemErrors.DueDateInPast);

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
                throw new DomainException(TaskItemErrors.NullLabel);

            if (_labels.Any(l => l.Id == label.Id))
                throw new DomainException(TaskItemErrors.DuplicateLabel);

            _labels.Add(label);

            MarkAsUpdated();
        }

        public void RemoveLabel(TaskLabel label)
        {
            if (label == null)
                throw new DomainException(TaskItemErrors.NullLabel);

            var labelToRemove = _labels.FirstOrDefault(l => l.Id == label.Id);
            if (labelToRemove == null)
                throw new DomainException(TaskItemErrors.LabelNotFound);

            _labels.Remove(labelToRemove);

            MarkAsUpdated();
        }

        public void RemoveLabelById(int labelId)
        {
            var label = _labels.FirstOrDefault(l => l.Id == labelId);
            if (label == null)
                throw new DomainException(TaskItemErrors.LabelNotFound);

            _labels.Remove(label);
            MarkAsUpdated();
        }

        public bool IsOverdue() => DueDate < DateOnly.FromDateTime(DateTime.Today);

        public bool IsComplete() => Status == Status.Completed;

        public bool HasLabel(int labelId) => _labels.Any(l => l.Id == labelId);
    }
}