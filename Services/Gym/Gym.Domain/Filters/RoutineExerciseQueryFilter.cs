using Core.Domain.Filters;

namespace Gym.Domain.Filters
{
    public class RoutineExerciseQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? RoutineId { get; private set; }
        public int? ExerciseId { get; private set; }
        public int? SetsLessEquals { get; private set; }
        public int? SetsLessThan { get; private set; }
        public int? SetsGreaterThan { get; private set; }
        public int? RepetitionsEquals { get; private set; }
        public int? RepetitionsLessThan { get; private set; }
        public int? RepetitionsGreaterThan { get; private set; }
        public int? RestTimeEquals { get; private set; }
        public int? RestTimeLessThan { get; private set; }
        public int? RestTimeGreaterThan { get; private set; }
        public int? RecommendedWeightEquals { get; private set; }
        public int? RecommendedWeightLessThan { get; private set; }
        public int? RecommendedWeightGreaterThan { get; private set; }
        public string? InstructionsContains { get; private set; }

        public RoutineExerciseQueryFilter(
            int? id = null,
            int? routineId = null,
            int? exerciseId = null,
            int? setsLessEquals = null,
            int? setsLessThan = null,
            int? setsGreaterThan = null,
            int? repetitionsEquals = null,
            int? repetitionsLessThan = null,
            int? repetitionsGreaterThan = null,
            int? restTimeEquals = null,
            int? restTimeLessThan = null,
            int? restTimeGreaterThan = null,
            int? recommendedWeightEquals = null,
            int? recommendedWeightLessThan = null,
            int? recommendedWeightGreaterThan = null,
            string? instructionsContains = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
        {
            Id = id;
            RoutineId = routineId;
            ExerciseId = exerciseId;
            SetsLessEquals = setsLessEquals;
            SetsLessThan = setsLessThan;
            SetsGreaterThan = setsGreaterThan;
            RepetitionsEquals = repetitionsEquals;
            RepetitionsLessThan = repetitionsLessThan;
            RepetitionsGreaterThan = repetitionsGreaterThan;
            RestTimeEquals = restTimeEquals;
            RestTimeLessThan = restTimeLessThan;
            RestTimeGreaterThan = restTimeGreaterThan;
            RecommendedWeightEquals = recommendedWeightEquals;
            RecommendedWeightLessThan = recommendedWeightLessThan;
            RecommendedWeightGreaterThan = recommendedWeightGreaterThan;
            InstructionsContains = instructionsContains;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
            SortBy = sortBy;
            SortDesc = sortDesc;
            Page = page;
            PageSize = pageSize;
        }
    }
}
