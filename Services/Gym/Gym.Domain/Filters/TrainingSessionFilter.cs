using Core.Domain.Filters;

namespace Gym.Domain.Filters
{
    public class TrainingSessionFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public int? UserId { get; private set; }
        public int? RoutineId { get; private set; }
        public DateOnly? StartTime { get; private set; }
        public DateOnly? EndTime { get; private set; }
        public string? NotesContains { get; private set; }
        public int? CompletedExerciseId { get; private set; }

        public TrainingSessionFilter(
            int? id = null,
            int? userId = null,
            int? routineId = null,
            DateOnly? startTime = null,
            DateOnly? endTime = null,
            string? notesContains = null,
            int? completedExerciseId = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
        {
            Id = id;
            UserId = userId;
            RoutineId = routineId;
            StartTime = startTime;
            EndTime = endTime;
            NotesContains = notesContains;
            CompletedExerciseId = completedExerciseId;
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
