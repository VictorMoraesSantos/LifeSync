using BuildingBlocks.CQRS.Requests.Commands;

namespace Financial.Application.Features.Categories.Commands.Delete
{
    public record DeleteCategoryCommand(int Id) : ICommand<DeleteCategoryResult>;
    public record DeleteCategoryResult(bool IsSuccess);
}
