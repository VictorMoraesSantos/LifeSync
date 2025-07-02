using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand, CreateCategoryResult>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<CreateCategoryResult>> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            var dto = new CreateCategoryDTO(command.UserId, command.Name, command.Description);
            var result = await _categoryService.CreateAsync(dto, cancellationToken);
            if (!result.IsSuccess)
                return Result<CreateCategoryResult>.Failure(result.Error!);

            return Result.Success(new CreateCategoryResult(result.Value!));
        }
    }
}
