using BuildingBlocks.CQRS.Request;

namespace Financial.Application.Features.Categories.Commands.Create
{
    public record CreateCategoryCommand(
        int UserId,
        string Name,
        string? Description)
        : IRequest<CreateCategoryResult>;
    public record CreateCategoryResult(int Id);
}
