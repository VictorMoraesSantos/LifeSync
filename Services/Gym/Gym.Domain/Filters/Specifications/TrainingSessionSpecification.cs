using Core.Domain.Filters;
using Gym.Domain.Entities;

namespace Gym.Domain.Filters.Specifications
{
    public class TrainingSessionSpecification : Specification<TrainingSession, int>
    {
        public TrainingSessionSpecification(TrainingSessionQueryFilter filter)
        {
            ApplyBaseFilters(filter);
            AddIf(filter.Id.HasValue, r => r.Id == filter.Id!.Value);
            AddIf(filter.UserId.HasValue, r => r.UserId == filter.UserId!.Value);
            AddIf(filter.RoutineId.HasValue, r => r.RoutineId == filter.RoutineId!.Value);
            AddIf(filter.StartTime.HasValue, r => DateOnly.FromDateTime(r.StartTime) >= filter.StartTime!.Value);
            AddIf(filter.EndTime.HasValue, r => DateOnly.FromDateTime(r.EndTime!.Value) <= filter.EndTime!.Value);
            AddIf(!string.IsNullOrWhiteSpace(filter.NotesContains), r => r.Notes.Contains(filter.NotesContains!));
            AddInclude(r => r.CompletedExercises);
        }
    }
}
