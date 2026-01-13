using Core.Application.DTO;
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
