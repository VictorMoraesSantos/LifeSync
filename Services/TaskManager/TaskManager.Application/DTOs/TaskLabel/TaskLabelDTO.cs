using Core.Application.DTO;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskLabel
{
    public record TaskLabelDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        LabelColor LabelColor,
        int UserId)
        : DTOBase(Id, CreatedAt, UpdatedAt);
}
