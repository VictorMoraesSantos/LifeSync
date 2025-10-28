namespace Core.Domain.Filters
{
    public interface IDomainQueryFilter<T>
    {
        T? Id { get; }
        DateOnly? CreatedAt { get; }
        DateOnly? UpdatedAt { get; }
        bool? IsDeleted { get; }
        string? SortBy { get; }
        bool? SortDesc { get; }
        int? Page { get; }
        int? PageSize { get; }
    }

    public class DomainQueryFilter<T> : IDomainQueryFilter<T>
    {
        public T? Id { get; private set; }
        public DateOnly? CreatedAt { get; private set; }
        public DateOnly? UpdatedAt { get; private set; }
        public bool? IsDeleted { get; private set; }
        public string? SortBy { get; private set; }
        public bool? SortDesc { get; private set; }
        public int? Page { get; private set; } = 1;
        public int? PageSize { get; private set; } = 50;

        public DomainQueryFilter(
            T? id,
            DateOnly? createdAt,
            DateOnly? updatedAt,
            bool? isDeleted,
            string? sortBy,
            bool? sortDesc,
            int? page,
            int? pageSize)
        {
            Id = id;
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
