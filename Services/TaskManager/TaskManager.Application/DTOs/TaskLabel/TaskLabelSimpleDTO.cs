using Core.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Application.DTOs.TaskItem;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskLabel
{
    public record TaskLabelSimpleDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        LabelColor Color,
        int UserId)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
