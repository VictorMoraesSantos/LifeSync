using System.Text.Json.Serialization;

namespace LifeSyncApp.Client.Models.TaskManager;

// Enums must match backend values
public enum Priority { Low = 1, Medium = 2, High = 3, Urgent = 4 }
public enum Status { Pending = 1, InProgress = 2, Completed = 3, Cancelled = 4 }
public enum LabelColor { Red = 0, Green, Blue, Yellow, Purple, Orange, Pink, Brown, Gray }

// Read DTOs
public record TaskLabelDTO(
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Name,
    LabelColor Color,
    int UserId,
    int? TaskItemId);

public record TaskItemDTO(
    int Id,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateOnly DueDate,
    int UserId,
    List<TaskLabelDTO> Labels);

// Commands (request payloads)
public record CreateTaskItemCommand(
    string Title,
    string Description,
    Priority Priority,
    DateOnly DueDate,
    int UserId);

public record UpdateTaskItemCommand(
    int Id,
    string Title,
    string Description,
    Status Status,
    Priority Priority,
    DateOnly DueDate);

public record CreateTaskLabelCommand(
    string Name,
    LabelColor LabelColor,
    int UserId,
    int TaskItemId);

public record UpdateTaskLabelCommand(
    int Id,
    string Name,
    LabelColor LabelColor);
