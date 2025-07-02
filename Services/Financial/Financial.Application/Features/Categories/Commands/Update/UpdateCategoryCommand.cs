using BuildingBlocks.CQRS.Commands;
using BuildingBlocks.CQRS.Request;
using System.Windows.Input;

namespace Financial.Application.Features.Categories.Commands.Update
{
    public record UpdateCategoryCommand(
        int Id,
        string Name,
        string? Description)
        : ICommand<UpdateCategoryResult>;
    public record UpdateCategoryResult(bool IsSuccess);
}
