using BuildingBlocks.CQRS.Handlers;
using BuildingBlocks.Results;
using Gym.Application.Contracts;

namespace Gym.Application.Features.CompletedExercise.Queries.GetCompletedExerciseById
{
    public class GetCompletedExerciseByIdQueryHandler : IQueryHandler<GetCompletedExerciseByIdQuery, CompletedExerciseResult>
    {
        private readonly ICompletedExerciseService _completedExerciseService;

        public GetCompletedExerciseByIdQueryHandler(ICompletedExerciseService completedExerciseService)
        {
            _completedExerciseService = completedExerciseService;
        }

        public async Task<Result<CompletedExerciseResult>> Handle(GetCompletedExerciseByIdQuery query, CancellationToken cancellationToken)
        {
            var result = await _completedExerciseService.GetByIdAsync(query.Id, cancellationToken);
            if (!result.IsSuccess)
                return Result.Failure<CompletedExerciseResult>(result.Error!);

            return Result.Success(new CompletedExerciseResult(result.Value!));
        }
    }
}
