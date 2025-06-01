using BuildingBlocks.CQRS.Request;

namespace Financial.Application.Features.Categories.Commands.Delete
{
    public record DeleteCategoryCommand(int Id) : IRequest<DeleteCategoryResult>;
    public record DeleteCategoryResult(bool IsSuccess);
}
