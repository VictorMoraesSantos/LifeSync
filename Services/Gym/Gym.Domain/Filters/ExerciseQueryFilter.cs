using Core.Domain.Filters;

namespace Gym.Domain.Filters
{
    public class ExerciseQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? NameContains { get; private set; }
        public string? DescriptionContains { get; private set; }
        public string? MuscleGroupContains { get; private set; }
        public string? TypeContains { get; private set; }
        public string? EquipamentTypeContains { get; private set; }

        public ExerciseQueryFilter(
            int? id,
            string? nameContains,
            string? descriptionContains,
            string? muscleGroupContains,
            string? typeContains,
            string? equipamentTypeContains,
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
            MuscleGroupContains = muscleGroupContains;
            TypeContains = typeContains;
            EquipamentTypeContains = equipamentTypeContains;
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
