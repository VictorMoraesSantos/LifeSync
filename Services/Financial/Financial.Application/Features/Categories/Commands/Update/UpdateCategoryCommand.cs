using MediatR;

namespace Financial.Application.Features.Categories.Commands.Update
{
    public record UpdateCategoryCommand(
        int Id,
        string Name,
        string? Description)
        : IRequest<UpdateCategoryResult>;
    public record UpdateCategoryResult(bool IsSuccess);
}
