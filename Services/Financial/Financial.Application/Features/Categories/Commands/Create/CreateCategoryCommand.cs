using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Queries;
using BuildingBlocks.CQRS.Request;

namespace Financial.Application.Features.Categories.Commands.Create
{
    public record CreateCategoryCommand(
        int UserId,
        string Name,
        string? Description)
        : ICommand<CreateCategoryResult>;
    public record CreateCategoryResult(int Id);
}
