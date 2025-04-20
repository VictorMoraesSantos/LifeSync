using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs
{
    public record TaskLabelDTO
    {
        public int Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public string Name { get; init; }
        public LabelColor Color { get; init; }
        public int UserId { get; init; }
        public int TaskItemId { get; init; }

        public TaskLabelDTO(int id, DateTime createdAt, DateTime? updatedAt, string name, LabelColor color, int userId, int taskItemId)
        {
            Id = id;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Name = name;
            Color = color;
            UserId = userId;
            TaskItemId = taskItemId;
        }
    }
}
