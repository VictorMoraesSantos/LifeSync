using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Categories.Queries.GetById
{
    public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, GetCategoryByIdResult>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryByIdQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<GetCategoryByIdResult>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<GetCategoryByIdResult>(result.Error!);

            return Result.Success(new GetCategoryByIdResult(result.Value!));
        }
    }
}
