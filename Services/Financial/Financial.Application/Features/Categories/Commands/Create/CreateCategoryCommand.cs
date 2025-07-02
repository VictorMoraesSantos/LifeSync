using BuildingBlocks.CQRS.Commands;

namespace Financial.Application.Features.Categories.Commands.Create
{
    public record CreateCategoryCommand(
        int UserId,
        string Name,
        string? Description)
        : ICommand<CreateCategoryResult>;
    public record CreateCategoryResult(int Id);
}
