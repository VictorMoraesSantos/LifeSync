using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Categories.Queries.GetByFilter
{
    public class GetCategoriesByFilterQueryHandler : IQueryHandler<GetCategoriesByFilterQuery, GetCategoriesByFilterResult>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesByFilterQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<GetCategoriesByFilterResult>> Handle(GetCategoriesByFilterQuery query, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByFilterAsync(query.Filter, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetCategoriesByFilterResult>(result.Error!);

            return Result.Success(new GetCategoriesByFilterResult(result.Value.Items, result.Value.Pagination));
        }
    }
}
