using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommandHandler : ICommandHandler<UpdateCategoryCommand, UpdateCategoryResult>
    {
        private readonly ICategoryService _categoryService;

        public UpdateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<UpdateCategoryResult>> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            var dto = new UpdateCategoryDTO(command.Id, command.Name, command.Description);
            var result = await _categoryService.UpdateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result<UpdateCategoryResult>.Failure(result.Error!);

            return Result.Success(new UpdateCategoryResult(result.Value!));
        }
    }
}
