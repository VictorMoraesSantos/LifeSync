using Financial.Application.Contracts;
using MediatR;

namespace Financial.Application.Features.Categories.Queries.GetAll
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, GetAllCategoriesResult>
    {
        private readonly ICategoryService _categoryService;

        public GetAllCategoriesQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<GetAllCategoriesResult> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
        {
            var categories = await _categoryService.GetAllAsync(cancellationToken);
            return new GetAllCategoriesResult(categories);
        }
    }
}
