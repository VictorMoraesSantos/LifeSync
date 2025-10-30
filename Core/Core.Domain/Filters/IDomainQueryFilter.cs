namespace Core.Domain.Filters
{
    public interface IDomainQueryFilter
    {
        DateOnly? CreatedAt { get; }
        DateOnly? UpdatedAt { get; }
        bool? IsDeleted { get; }
        string? SortBy { get; }
        bool? SortDesc { get; }
        int? Page { get; }
        int? PageSize { get; }
    }

    public abstract class DomainQueryFilter : IDomainQueryFilter
    {
        public DateOnly? CreatedAt { get; protected set; }
        public DateOnly? UpdatedAt { get; protected set; }
        public bool? IsDeleted { get; protected set; }
        public string? SortBy { get; protected set; }
        public bool? SortDesc { get; protected set; }
        public int? Page { get; protected set; } = 1;
        public int? PageSize { get; protected set; } = 50;
    }
}
