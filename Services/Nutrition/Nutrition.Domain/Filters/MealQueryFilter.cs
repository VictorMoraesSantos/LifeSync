using Core.Domain.Filters;

namespace Nutrition.Domain.Filters
{
    public class MealQueryFilter : DomainQueryFilter
    {
        public int? Id { get; private set; }
        public string? NameContains { get; private set; }
        public string? DescriptionContains { get; private set; }
        public int? DiaryId { get; private set; }
        public int? TotalCaloriesEqual { get; private set; }
        public int? TotalCaloriesGreaterThen { get; private set; }
        public int? TotalCaloriesLessThen { get; private set; }
        public int? MealFoodId { get; private set; }

        public MealQueryFilter(
            int? id = null,
            string? nameContains = null,
            string? descriptionContains = null,
            int? diaryId = null,
            int? totalCaloriesEqual = null,
            int? totalCaloriesGreaterThen = null,
            int? totalCaloriesLessThen = null,
            int? mealFoodId = null,
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
            DiaryId = diaryId;
            TotalCaloriesEqual = totalCaloriesEqual;
            TotalCaloriesGreaterThen = totalCaloriesGreaterThen;
            TotalCaloriesLessThen = totalCaloriesLessThen;
            MealFoodId = mealFoodId;
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
