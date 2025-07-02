using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.CQRS.Request;
using BuildingBlocks.Results;
using Financial.Application.Contracts;

namespace Financial.Application.Features.Categories.Queries.GetAll
{
    public class GetAllCategoriesQueryHandler : IQueryHandler<GetAllCategoriesQuery, GetAllCategoriesResult>
    {
        private readonly ICategoryService _categoryService;

        public GetAllCategoriesQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<Result<GetAllCategoriesResult>> Handle(GetAllCategoriesQuery query, CancellationToken cancellationToken)
        {
            var result = await _categoryService.GetAllAsync(cancellationToken);
            if (!result.IsSuccess)
                return Result<GetAllCategoriesResult>.Failure(result.Error!);

            return Result.Success(new GetAllCategoriesResult(result.Value!));
        }
    }
}
