using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;
using Financial.Application.DTOs.Category;

namespace Financial.Application.Features.Categories.Commands.Create
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResult>
    {
        private readonly ICategoryService _categoryService;

        public CreateCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<CreateCategoryResult> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            CreateCategoryDTO dto = new(command.UserId, command.Name, command.Description);
            var result = await _categoryService.CreateAsync(dto, cancellationToken);
            return new CreateCategoryResult(result);
        }
    }
}
