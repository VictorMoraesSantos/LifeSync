using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Nutrition.Application.Contracts;

namespace Nutrition.Application.Features.Food.Queries.GetById
{
    public class GetFoodByIdQueryHandler : IQueryHandler<GetFoodByIdQuery, GetFoodByIdResult>
    {
        private readonly IFoodService _foodService;

        public GetFoodByIdQueryHandler(IFoodService foodService)
        {
            _foodService = foodService;
        }

        public async Task<Result<GetFoodByIdResult>> Handle(GetFoodByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _foodService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess) return Result.Failure<GetFoodByIdResult>(result.Error!);

            return Result.Success(new GetFoodByIdResult(result.Value!));
        }
    }
}
