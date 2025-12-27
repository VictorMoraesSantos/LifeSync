using Core.Domain.Entities;
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
            SetPriority(priority);
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
            ClearErrors();

            SetTitle(title);
            SetDescription(description);
            Status = status;
            SetPriority(priority);
            DueDate = dueDate;
            MarkAsUpdated();
        }

        public void SetTitle(string title)
        {
            if (title == null || string.IsNullOrWhiteSpace(title) || title.Length < 3)
                AddError(TaskItemErrors.InvalidTitle);

            Title = title;
        }

        public void SetDescription(string description)
        {
            if (description == null || string.IsNullOrWhiteSpace(description) || description.Length < 5)
                AddError(TaskItemErrors.InvalidDescription);

            Description = description;
        }

        public void SetPriority(Priority priority)
        {
            if (!Enum.IsDefined(typeof(Priority), priority))
                AddError(TaskItemErrors.InvalidPriority);

            Priority = priority;
        }

        public void SetDueDate(DateOnly dueDate)
        {
            if (dueDate < DateOnly.FromDateTime(DateTime.UtcNow))
                AddError(TaskItemErrors.DueDateInPast);

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
                AddError(TaskItemErrors.NullLabel);

            if (_labels.Any(l => l.Id == label.Id))
                AddError(TaskItemErrors.DuplicateLabel);

            _labels.Add(label);

            MarkAsUpdated();
        }

        public void RemoveLabel(TaskLabel label)
        {
            if (label == null)
                AddError(TaskItemErrors.NullLabel);

            var labelToRemove = _labels.FirstOrDefault(l => l.Id == label.Id);
            if (labelToRemove == null)
                AddError(TaskItemErrors.LabelNotFound);

            _labels.Remove(labelToRemove);

            MarkAsUpdated();
        }

        public bool IsOverdue() => DueDate < DateOnly.FromDateTime(DateTime.UtcNow);

        public bool IsComplete() => Status == Status.Completed;

        public bool HasLabel(int labelId) => _labels.Any(l => l.Id == labelId);
    }
}