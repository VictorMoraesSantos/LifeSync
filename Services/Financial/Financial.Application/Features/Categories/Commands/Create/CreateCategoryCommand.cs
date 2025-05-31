using MediatR;

namespace Financial.Application.Features.Categories.Commands.Create
{
    public record CreateCategoryCommand(
        int UserId,
        string Name,
        string? Description)
        : IRequest<CreateCategoryResult>;
    public record CreateCategoryResult(int Id);
}
