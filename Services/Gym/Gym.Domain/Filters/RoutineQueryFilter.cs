using Core.Domain.Filters;

namespace Gym.Domain.Filters
{
    public class RoutineQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? NameContains { get; private set; }
        public string? DescriptionContains { get; private set; }
        public int? RoutineExerciseId { get; private set; }

        public RoutineQueryFilter(
            int? id = null,
            string? nameContains = null,
            string? descriptionContains = null,
            int? routineExerciseId = null,
            DateOnly? createdAt = null,
            DateOnly? updatedAt = null,
            bool? isDeleted = null,
            string? sortBy = null,
            bool? sortDesc = null,
            int? page = null,
            int? pageSize = null)
        {
            Id = id;
            NameContains = nameContains;
            DescriptionContains = descriptionContains;
            RoutineExerciseId = routineExerciseId;
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
