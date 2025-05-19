using Core.Application.DTO;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs.TaskLabel
{
    public record TaskLabelDTO(
        int Id,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string Name,
        LabelColor Color,
        int UserId,
        int TaskItemId
        ) : DTOBase(Id, CreatedAt, UpdatedAt);
}
