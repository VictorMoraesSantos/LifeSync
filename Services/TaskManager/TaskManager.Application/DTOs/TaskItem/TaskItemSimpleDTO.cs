using Core.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.DTOs.TaskLabel;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskItem
{
    public record TaskItemSimpleDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Title,
        string Description,
        Status Status,
        Priority Priority,
        DateOnly DueDate,
        int UserId)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
