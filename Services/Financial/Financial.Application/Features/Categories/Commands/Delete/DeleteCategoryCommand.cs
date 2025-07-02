using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;
using System.Windows.Input;

namespace Financial.Application.Features.Categories.Commands.Delete
{
    public record DeleteCategoryCommand(int Id) : ICommand<DeleteCategoryResult>;
    public record DeleteCategoryResult(bool IsSuccess);
}
