using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Categories.Queries.GetByUserId
{
    public class GetCategoriesByUserIdQueryHandler : IQueryHandler<GetCategoriesByUserIdQuery, GetCategoriesByUserIdResult>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesByUserIdQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<GetCategoriesByUserIdResult>> Handle(GetCategoriesByUserIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByUserIdAsync(query.UserId, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetCategoriesByUserIdResult>(result.Error!);

            return Result.Success(new GetCategoriesByUserIdResult(result.Value!));
        }
    }
}
