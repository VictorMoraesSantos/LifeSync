using Financial.Application.Contracts;
using MediatR;

namespace Financial.Application.Features.Categories.Queries.GetById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, GetCategoryByIdResult>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryByIdQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<GetCategoryByIdResult> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByIdAsync(query.Id, cancellationToken);
            return new GetCategoryByIdResult(result);
        }
    }
}
