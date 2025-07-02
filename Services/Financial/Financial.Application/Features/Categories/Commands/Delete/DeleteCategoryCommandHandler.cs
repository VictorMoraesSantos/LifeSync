using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Categories.Commands.Delete
{
    public class DeleteCategoryCommandHandler : ICommandHandler<DeleteCategoryCommand, DeleteCategoryResult>
    {
        private readonly ICategoryService _categoryService;

        public DeleteCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<DeleteCategoryResult>> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            var result = await _categoryService.DeleteAsync(command.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result<DeleteCategoryResult>.Failure(result.Error!);

            return Result.Success(new DeleteCategoryResult(result.Value!));
        }
    }
}
