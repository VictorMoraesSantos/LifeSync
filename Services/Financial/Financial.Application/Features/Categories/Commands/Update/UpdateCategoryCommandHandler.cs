using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Commands.Update
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResult>
    {
        private readonly ICategoryService _categoryService;

        public UpdateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<UpdateCategoryResult> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            UpdateCategoryDTO dto = new(command.Id, command.Name, command.Description);
            var result = await _categoryService.UpdateAsync(dto, cancellationToken);
            return new UpdateCategoryResult(result);
        }
    }
}
