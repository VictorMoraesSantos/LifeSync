using MediatR;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.TaskLabels.Queries.GetById
{
    public record GetByIdQuery(int Id) : IRequest<TaskLabelDTO?>;
}
