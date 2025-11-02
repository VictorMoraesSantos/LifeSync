using Core.Domain.Filters;

namespace Gym.Domain.Filters
{
    public class CompletedExerciseQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? TrainingSessionId { get; private set; }
        public int? RoutineExerciseId { get; private set; }
        public int? SetsCompletedEquals { get; private set; }
        public int? SetsCompletedLessThan { get; private set; }
        public int? SetsCompletedGreaterThan { get; private set; }
        public int? RepetitionsCompletedEquals { get; private set; }
        public int? RepetitionsCompletedLessThan { get; private set; }
        public int? RepetitionsCompletedGreaterThan { get; private set; }
        public int? WeightUsedCompletedEquals { get; private set; }
        public int? WeightUsedCompletedLessThan { get; private set; }
        public int? WeightUsedCompletedGreaterThan { get; private set; }
        public DateOnly CompletedAt { get; private set; }
        public string? NotesContains { get; private set; }

        public CompletedExerciseQueryFilter(
            int? id,
            int? trainingSessionId = null,
            int? routineExerciseId = null,
            int? setsCompletedEquals = null,
            int? setsCompletedLessThan = null,
            int? setsCompletedGreaterThan = null,
            int? repetitionsCompletedEquals = null,
            int? repetitionsCompletedLessThan = null,
            int? repetitionsCompletedGreaterThan = null,
            int? weightUsedCompletedEquals = null,
            int? weightUsedCompletedLessThan = null,
            int? weightUsedCompletedGreaterThan = null,
            DateOnly? completedAt = null,
            string? notesContains = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
        {
            Id = id;
            TrainingSessionId = trainingSessionId;
            RoutineExerciseId = routineExerciseId;
            SetsCompletedEquals = setsCompletedEquals;
            SetsCompletedLessThan = setsCompletedLessThan;
            SetsCompletedGreaterThan = setsCompletedGreaterThan;
            RepetitionsCompletedEquals = repetitionsCompletedEquals;
            RepetitionsCompletedLessThan = repetitionsCompletedLessThan;
            RepetitionsCompletedGreaterThan = repetitionsCompletedGreaterThan;
            WeightUsedCompletedEquals = weightUsedCompletedEquals;
            WeightUsedCompletedLessThan = weightUsedCompletedLessThan;
            WeightUsedCompletedGreaterThan = weightUsedCompletedGreaterThan;
            CompletedAt = completedAt ?? default;
            NotesContains = notesContains;
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
