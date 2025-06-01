using BuildingBlocks.CQRS.Request;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Categories.Queries.GetByUserId
{
    public class GetCategoriesByUserIdQueryHandler : IRequestHandler<GetCategoriesByUserIdQuery, GetCategoriesByUserIdResult>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesByUserIdQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<GetCategoriesByUserIdResult> Handle(GetCategoriesByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByUserIdAsync(query.UserId, cancellationToken);
            return new GetCategoriesByUserIdResult(result);
        }
    }
}
